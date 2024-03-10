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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace project_comp1640_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context) => _context = context;

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
            user.role = user.role;

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
                sb.Append("Password should be Alphanumeric" + Environment.NewLine);
            if (!Regex.IsMatch(password, "[<,>,@,!,#,$,$,^,&,*,(,),_,+,\\,//]"))
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
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, acc.role.ToString()),
                new Claim(ClaimTypes.Name, $"{acc.user_username}")
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

        [HttpPost("Update-Profile")]
        public async Task<IActionResult> UpdateProfile(Users user)
        {
            if(user == null)
            {
                return BadRequest(new {Message = "Data is null"});
            }

            _context.Entry(user).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            
            return Ok(new
            {
                Message = "Update Profile Succced"
            });
        }
    }
}
