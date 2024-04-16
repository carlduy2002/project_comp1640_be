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

        //just change 22/3/2024
        [HttpGet]
        //[Authorize(Policy = "Admin")]
        public async Task<IActionResult> getAllUsers()
        {
            var lstUsers = _context.Users
                .Include(u => u.role)
                .Include(f => f.faculties)
                .Select(u => new
                {
                    user_id = u.user_id,
                    user_username = u.user_username,
                    user_email = u.user_email,
                    user_faculty = u.faculties.faculty_name,
                    role_name = u.role.role_name,
                    user_status = u.user_status,
                    user_faculty_name = u.faculties.faculty_name
                })
                .ToList();

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
            user.user_avatar = "avt.png";
            user.total_work_duration = 0;

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

            if (!(Regex.IsMatch(email, "[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+.[a-zA-Z]{2,}")))
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
            var lastLogin = user.last_login;
            await _context.SaveChangesAsync();

            //var time = DateTime.UtcNow;

            //addTotalWorkDuration(int.Parse(time), username);

            return Ok(new TokenApiDto()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                Message = "Login Succeed"
            });
            //return Ok();
        }

        //22/3/2024
        [HttpPut("check-old-password")]
        public async Task<IActionResult> checkOldPassword(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(a => a.user_username == username);

            if (user == null)
                return NotFound(new { Message = "User not found!" });

            if (!PasswordHasher.VerifyPassword(password, user.user_password))
                return BadRequest(new { Message = "Password is incorrectly!" });

            return Ok();
        }

        //22/3/2024
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword(string newPass, string conPass, string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.user_username == username);

            if (user == null)
                return NotFound(new { Message = "User not found!" });

            var pwd = CheckPasswordValid(newPass);
            if (!string.IsNullOrEmpty(pwd))
                return BadRequest(new { Message = pwd.ToString() });

            if (PasswordHasher.VerifyPassword(newPass, user.user_password))
                return BadRequest(new { Message = "This password already exists before, please enter another password!" });

            if (conPass != newPass)
                return BadRequest(new { Message = "Password and Confirm Password is not match!" });

            user.user_status = user_status.Unlock;
            user.user_password = PasswordHasher.HashPassword(newPass);
            user.user_confirm_password = PasswordHasher.HashPassword(conPass);

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Change Password Succeed"
            });
        }

        private string CreateJwt(Users acc)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("aaaaaaaaaaaaaaaa");

            var user = _context.Roles
            .Join(_context.Users.Where(u => u.user_username == acc.user_username),
                role => role.role_id,
                user => user.role.role_id,
                (role, user) => new
                {
                    role_name = role.role_name,
                    name = user.user_username
                })
            .ToList();

            string roleName = "";
            string userName = "";

            foreach (var item in user)
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
            var emailTokenExpiry = user.reset_password_exprytime;
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

        [HttpGet("get-user-id")]
        public async Task<IActionResult> getUserID(string user_username)
        {
            var userID = await _context.Users.Where(u => u.user_username.Equals(user_username)).Select(u => u.user_id).FirstOrDefaultAsync();

            return Ok(userID);
        }

        [HttpGet("Get-User-By-UserName")]
        public async Task<IActionResult> getUserByUserName(string user_username)
        {
            var user = await _context.Users.Where(u => u.user_username == user_username).Select(u => new {
                u.user_id, u.user_username, u.user_email, u.user_avatar, u.user_faculty_id, u.faculties.faculty_name, u.user_role_id, u.role.role_name, u.user_status
            }).FirstOrDefaultAsync();

            if(user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        private bool IsImageFile(string fileName)
        {
            string fileType = Path.GetExtension(fileName);
            return fileType.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                   fileType.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                   fileType.Equals(".png", StringComparison.OrdinalIgnoreCase);
        }

        private async Task<string> SaveFileAsync(IFormFile file, string directory)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), directory, fileName);

            // check file exiting
            int i = 0;
            while (System.IO.File.Exists(filePath))
            {
                i++;
                fileName = "(" + i + ")" + fileName;
                filePath = Path.Combine(Directory.GetCurrentDirectory(), directory, fileName);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }

        [HttpPost("Update-User-Profile")]
        public async Task<IActionResult> UpdateUserProfile()
        {
            var httpRequest = Request.Form;
            var userId = int.Parse(httpRequest["userId"]);
            var userName = httpRequest["userName"];
            var userEmail = httpRequest["userEmail"];
            var userAvatar = httpRequest.Files["uploadImage"];
            var userFaculty = int.Parse(httpRequest["userFaculty"]);
            var userRole = int.Parse(httpRequest["userRole"]);
            var userStatus = httpRequest["userStatus"];

            Users user = _context.Users.Where(u => u.user_id == userId).FirstOrDefault();

            if (user == null)
            {
                return BadRequest(new { Message = "User not found" });
            }

            if (userAvatar != null)
            {
                if (IsImageFile(userAvatar.FileName))
                {
                    SaveFileAsync(userAvatar, "Imgs");
                    user.user_avatar = userAvatar.FileName;
                }
            }

            if(user.user_email != userEmail)
            {
                if (await CheckEmailExist(userEmail))
                    return BadRequest(new { Message = "Email already exist!" });

                var email = CheckEmailValid(userEmail);
                if (!string.IsNullOrEmpty(email))
                    return BadRequest(new { Message = email.ToString() });
            }

            if (user.user_username != userName)
            {
                if (await CheckUsernameExist(userName))
                {
                    return BadRequest(new { Message = "Username already exist!" });
                }
            }

            user.user_username = userName;
            user.user_email = userEmail;
            user.user_faculty_id = userFaculty;
            user.user_role_id = userRole;
            user.user_status = user_status.Unlock;
            if (userStatus.Equals("Lock"))
            {
                user.user_status = user_status.Lock;
            }

            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();

            user = _context.Users.Where(u => u.user_id == userId).FirstOrDefault();

            user.token = CreateJwt(user);
            var newAccessToken = user.token;
            var newRefreshToken = CreateRefreshToken();
            user.refesh_token = newRefreshToken;
            user.refesh_token_exprytime = DateTime.Now.AddDays(1);
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
            return Ok(new TokenApiDto()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                Message = "Succeed"
            });
        }

        [HttpGet("last-login")]
        public async Task<IActionResult> lastLogin(string username)
        {
            if(username == null) { return BadRequest(new { Message = "Data provided is null" }); }

            var user = _context.Users.Where(u => u.user_username.Equals(username)).FirstOrDefault();

            if(user == null) { return BadRequest(new { Message = "User is not found" }); }

            return Ok(user.last_login);
        }

        [HttpPost("add-last-login")]
        public async Task<IActionResult> AddLastLogin(string username)
        {
            if (username == null) { return BadRequest(new { Message = "Data provided is null" }); }

            var user = _context.Users.Where(u => u.user_username == username).FirstOrDefault();

            if (user == null) { return BadRequest(new { Message = "User is not found" }); }

            user.last_login = DateTime.Now;

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new {Message = "Add last login successfully"});
        }

        [HttpPost("add-total-work-duration")]
        public async Task<IActionResult> addTotalWorkDurationgit(int time, string username)
        {
            if(username == null || time == null) { return BadRequest(new {Message = "Data provided is null"}); }

            var user = _context.Users.Where(u => u.user_username == username).FirstOrDefault();

            if (user == null) { return BadRequest(new { Message = "User is not found" }); }

            user.total_work_duration += time;

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new {Message = "Add total work duration successfully"});
        }

        //[HttpGet("last-login")]
        //public async Task<IActionResult> lastLogin(string username)
        //{
        //    if (username == null) { return BadRequest(new { Message = "Data provided is null" }); }

        //    var user = _context.Users.Where(u => u.user_username == username).FirstOrDefault();

        //    if (user == null) { return BadRequest(new { Message = "User is not found" }); }

        //    return Ok(user.last_login);
        //}

        //[HttpPost("add-last-login")]
        //public async Task<IActionResult> AddLastLogin(string username)
        //{
        //    if (username == null) { return BadRequest(new { Message = "Data provided is null" }); }

        //    var user = _context.Users.Where(u => u.user_username == username).FirstOrDefault();

        //    if (user == null) { return BadRequest(new { Message = "User is not found" }); }

        //    user.last_login = DateTime.Now;

        //    _context.Entry(user).State = EntityState.Modified;
        //    await _context.SaveChangesAsync();

        //    return Ok(new { Message = "Add last login successfully" });
        //}

        //[HttpPost("add-total-work-duration")]
        //public async Task<IActionResult> addTotalWorkDuration(int time, string username)
        //{
        //    if (username == null || time == null) { return BadRequest(new { Message = "Data provided is null" }); }

        //    var user = _context.Users.Where(u => u.user_username == username).FirstOrDefault();

        //    if (user == null) { return BadRequest(new { Message = "User is not found" }); }

        //    user.total_work_duration += time;

        //    _context.Entry(user).State = EntityState.Modified;
        //    await _context.SaveChangesAsync();

        //    return Ok(new { Message = "Add total work duration successfully" });
        //}
    }
}
