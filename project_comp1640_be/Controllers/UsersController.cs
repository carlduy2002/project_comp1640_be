using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_comp1640_be.Model;
using System.Text.RegularExpressions;
using System.Text;
using project_comp1640_be.Data;
using project_comp1640_be.Helper;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using project_comp1640_be.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using NETCore.MailKit.Core;
using IEmailService = project_comp1640_be.UtilityService.IEmailService;

namespace project_comp1640_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _env;

        public UsersController(ApplicationDbContext context, IConfiguration configuration, IEmailService emailService, IWebHostEnvironment env)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
            _env = env;
        }

        [HttpGet]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> getAllUsers()
        {
            var lstUsers = _context.Users.ToList();

            return Ok(lstUsers);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] Users user)
        {
            if (user == null)
                return BadRequest();

            if (await CheckUsernameExist(user.user_username))
                return BadRequest(new { Message = "Username already exist!" });

            if (await CheckEmailExist(user.user_email))
                return BadRequest(new { Message = "Email already exist!" });

            var email = CheckEmailValid(user.user_email);
            if (!string.IsNullOrEmpty(email))
                return BadRequest(new { Message = email.ToString() });

            var pwd = CheckPasswordValid(user.user_password);
            if (!string.IsNullOrEmpty(pwd))
                return BadRequest(new { Message = pwd.ToString() });

            if (user.user_confirm_password != user.user_password)
                return BadRequest(new { Message = "Password and Confirm Password is not match!" });

            user.user_password = PasswordHasher.HashPassword(user.user_password);
            user.user_confirm_password = PasswordHasher.HashPassword(user.user_confirm_password);
            user.user_status = user_status.Unlock;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Register Succeed"
            });
        }

        //Check username that exist or not
        private Task<bool> CheckUsernameExist(string username)
        {
            return _context.Users.AnyAsync(x => x.user_username == username);
        }

        //Check email that exist or not
        private Task<bool> CheckEmailExist(string email)
        {
            return _context.Users.AnyAsync(x => x.user_email == email);
        }

        private string CheckEmailValid(string email)
        {
            StringBuilder sb = new StringBuilder();

            if (!(Regex.IsMatch(email, "[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}")))
                sb.Append("Email is invalid" + Environment.NewLine);

            return sb.ToString();
        }

        private string CheckPasswordValid(string password)
        {
            StringBuilder sb = new StringBuilder();
            if (password.Length < 8)
                sb.Append("Minimum password length should be 8" + Environment.NewLine);
            if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]")
                && Regex.IsMatch(password, "[0-9]")))
                sb.Append("Password should be Alphanumeric and have number" + Environment.NewLine);
            if (!Regex.IsMatch(password, "[<,>,@,!,#,$,$,^,&,*,(,),_,+]"))
                sb.Append("Password should contain special chars" + Environment.NewLine);

            return sb.ToString();
        }


        //Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (username == null && password == null)
                return BadRequest();

            var user = await _context.Users.FirstOrDefaultAsync(x => x.user_username == username);

            if (user == null)
                return NotFound(new { Message = "User Not Found!" });

            if (user.user_status == user_status.Lock)
                return BadRequest(new { Message = "Account has been locked!!" });

            if (!PasswordHasher.VerifyPassword(password, user.user_password))
                return BadRequest(new { Message = "Password is incorrectly!" });

            user.token = CreateJwt(user);
            var newAccessToken = user.token;
            var newRefreshToken = CreateRefreshToken();
            user.refesh_token = newRefreshToken;
            user.refesh_token_exprytime = DateTime.Now.AddDays(1);
            await _context.SaveChangesAsync();

            return Ok(new TokenApiDto()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                Message = "Login Succeed"
            });
        }

        private string CreateJwt(Users acc)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("aaaaaaaaaaaaaaaa");

            var user = _context.Roles
            .Join(_context.Users.Where(u => u.user_username == acc.user_username),
                role => role.role_id,
                user => user.user_role_id,
                (role, user) => new
                {
                    role_name = role.role_name,
                    name = user.user_username
                })
            .ToList();

            string roleName = "";
            string userName = "";

            foreach(var item in user)
            {
                roleName = item.role_name;
                userName = item.name;
            }

            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role,roleName),
                new Claim(ClaimTypes.Name, $"{userName}")
            });

            var credential = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddSeconds(10),
                SigningCredentials = credential
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            return jwtTokenHandler.WriteToken(token);
        }


        private string CreateRefreshToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var refreshToken = Convert.ToBase64String(tokenBytes);

            var tokenInUser = _context.Users.Any(a => a.refesh_token == refreshToken);
            if (tokenInUser)
            {
                return CreateRefreshToken();
            }

            return refreshToken;
        }

        [HttpPost("send-email/{email}")]
        public async Task<IActionResult> SendEmail(string email)
        {
            var user = _context.Users.FirstOrDefault(x => x.user_email == email);
            if (user is null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "email doesn't exist"
                });
            }

            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var emailToken = Convert.ToBase64String(tokenBytes);
            user.reset_password_token = emailToken;
            user.reset_password_exprytime = DateTime.Now.AddMinutes(15);

            string from = _configuration["EmailSettings:From"];
            var emailModel = new EmailModel(email, "Reset Password!!", EmailBody.EmailStringBody(email, emailToken));
            _emailService.SendEmail(emailModel);
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                StatusCode = 200,
                Message = "Email Sent!, Please check your email!"
            });
        }



        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDto)
        {
            var newToken = resetPasswordDto.EmailToken.Replace(" ", "+");
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.user_email == resetPasswordDto.Email);
            if (user == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Email doesn't exist"
                });
            }

            var tokenCode = user.reset_password_token;
            DateTime emailTokenExpiry = user.reset_password_exprytime;
            if (tokenCode != resetPasswordDto.EmailToken || emailTokenExpiry < DateTime.Now)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Invalid Reset Link"
                });
            }

            var pwd = CheckPasswordValid(resetPasswordDto.NewPassword);
            if (!string.IsNullOrEmpty(pwd))
                return BadRequest(new { Message = pwd.ToString() });

            if (PasswordHasher.VerifyPassword(resetPasswordDto.NewPassword, user.user_password))
                return BadRequest(new { Message = "This password already exists before, please enter another password!" });

            user.user_password = PasswordHasher.HashPassword(resetPasswordDto.NewPassword);
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                StatusCode = 200,
                Message = "Reset Password Successfully"
            });
        }
    }
}
