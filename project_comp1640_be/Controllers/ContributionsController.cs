﻿using Aspose.Words.Saving;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.Extensions.Configuration;
using NETCore.MailKit.Core;
using project_comp1640_be.Data;
using project_comp1640_be.Helper;
using project_comp1640_be.Model;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using IEmailService = project_comp1640_be.UtilityService.IEmailService;
using Aspose.Words;
using Microsoft.AspNetCore.Html;


namespace project_comp1640_be.Controllers
{
    [Route("contribution")]
    [ApiController]
    public class ContributionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _env;

        public ContributionsController(ApplicationDbContext context, IConfiguration configuration, IEmailService emailService, IWebHostEnvironment env)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
            _env = env;
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

        // check type word
        private bool IsWordFile(string fileName)
        {
            string fileType = Path.GetExtension(fileName);
            return fileType.Equals(".doc", StringComparison.OrdinalIgnoreCase) || fileType.Equals(".docx", StringComparison.OrdinalIgnoreCase);
        }

        // check type img
        private bool IsImageFile(string fileName)
        {
            string fileType = Path.GetExtension(fileName);
            return fileType.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                   fileType.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                   fileType.Equals(".png", StringComparison.OrdinalIgnoreCase);
        }
        private void DeleteFile(string fileName, string directory)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), directory, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
        // send email function
        private void SendEmail(string email)
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var emailToken = Convert.ToBase64String(tokenBytes);

            string from = _configuration["EmailSettings:From"];
            var emailBody = EmailBody.AddNewArticleEmailStringBody();
            var emailModel = new EmailModel(email, "New ariticle posted!!", emailBody);
            _emailService.SendEmail(emailModel);
        }

        //load file word to html
        [HttpGet("Load-file")]
        public async Task<IActionResult> LoadWordFile(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Articles", fileName);

            if (System.IO.File.Exists(filePath))
            {
                Document doc = new Document(filePath);

                // doc.RemoveMacros();

                MemoryStream stream = new MemoryStream();

                HtmlSaveOptions options = new HtmlSaveOptions();
                options.ExportImagesAsBase64 = true;

                doc.Save(stream, options);

                stream.Position = 0;

                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string htmlContent = reader.ReadToEnd();

                htmlContent = Regex.Replace(htmlContent, @"(Evaluation Only\. Created with Aspose\.Words\. Copyright \d{4}-\d{4} Aspose Pty Ltd\.|Created with an evaluation copy of Aspose\.Words\. To discover the full versions of our APIs please visit: https://products\.aspose\.com/words/)", string.Empty);

                return Content(htmlContent, "text/html");
            }

            return NotFound("File not found");
        }

        [HttpPost("Add-New-Article")]
        public async Task<IActionResult> AddNewArticle()
        {
            try
            {
                Contributions con = new Contributions();

                var httpRequest = Request.Form;

                var title = Request.Form["title"];

                var username = Request.Form["username"];

                var article = httpRequest.Files["uploadFile"];

                var thumbnailImg = httpRequest.Files["uploadImage"];

                if (IsWordFile(article.FileName) && IsImageFile(thumbnailImg.FileName))
                {
                    // upload article
                    con.contribution_content = await SaveFileAsync(article, "Articles");
                    // upload tumbnail img
                    con.contribution_image = await SaveFileAsync(thumbnailImg, "Imgs");
                }
                else
                {
                    return BadRequest(new { Message = "File Format Not Supported" });
                }

                // add academic year
                var date = DateTime.Now;
                var academicyear = _context.Academic_Years.Where(a => a.academic_year_startClosureDate >= date).Select(a => a.academic_year_id).FirstOrDefault();
                con.contribution_academic_years_id = academicyear;

                var user = _context.Users.Where(u => u.user_username.Equals(username)).FirstOrDefault();
                var userId = user.user_id;
                con.contribution_user_id = userId;

                con.contribution_title = title;
                con.contribution_submition_date = date;
                con.IsEnabled = IsEnabled.Enabled;
                con.IsSelected = IsSelected.Unselected;
                con.IsView = IsView.Unview;

                _context.Contributions.Add(con);
                await _context.SaveChangesAsync();

                // get user faculty
                var userFaculty = _context.Users.Where(u => u.user_faculty_id.Equals(username)).Select(u => u.user_id).FirstOrDefault();

                // fine faculty manager and send email
                var maketingCondinatorUser = _context.Users.Where(u => u.user_faculty_id == user.user_faculty_id && u.user_role_id == 3).FirstOrDefault();
                var maketingCondinatorEmail = maketingCondinatorUser.user_email;
                SendEmail(maketingCondinatorEmail);

                return Ok(new { Message = "Add article succeeded" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Upload File Not Succeed - " + " Error: " + ex });
            }
        }

        [HttpGet("Get-Article")]
        public async Task<IActionResult> GetArticleById(int contribution_id)
        {
            if (contribution_id == null)
            {
                return BadRequest(new { Message = "Data is null" });
            }

            var contribution = await _context.Contributions.FindAsync(contribution_id);
            if (contribution == null)
            {
                return NotFound(new { Message = "Couldn't find" });
            }
            return Ok(contribution);
        }

        [HttpGet("Get-All-Articles")]
        public async Task<IActionResult> GetAllArticles()
        {
            var contributions = await _context.Contributions.ToListAsync();
            return Ok(contributions);
        }

        [HttpGet("student_id")]
        public async Task<IActionResult> getContributionStudent(int student_id)
        {
            var student = _context.Contributions.Where(c => c.contribution_user_id == student_id).ToList();

            if (student == null)
                return BadRequest();

            return Ok(student);

        }

        [HttpGet("delete-contribution")]
        public async Task<IActionResult> deleteContribution(int contribution_id)
        {
            if (contribution_id == null) { return BadRequest(new { Message = "Data is provided is null" }); }

            var contribution = await _context.Contributions.FirstOrDefaultAsync(c => c.contribution_id == contribution_id);

            if (contribution == null) { return BadRequest(new { Message = "Contribution is not found" }); }

            contribution.IsEnabled = IsEnabled.Unenabled;

            _context.Entry(contribution).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Contribution is deleted" });
        }

        [HttpPut]
        public async Task<IActionResult> updateContribution(Contributions contributions)
        {
            if (contributions == null)
                return BadRequest(new { Message = "Contribution is null" });

            _context.Contributions.Entry(contributions).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Update Contribution Succeed" });
        }

        [HttpPut("update-thumbnail")]
        public async Task<IActionResult> updateThumbnail(int contribution_id)
        {
            if (contribution_id == 0 || contribution_id == null) return BadRequest(new { Message = "Contribution ID is null" });

            var file = Request.Form.Files["uploadImage"];

            if (file == null) return BadRequest(new { Message = "image file is null" });

            var contribution = _context.Contributions.FirstOrDefault(c => c.contribution_id == contribution_id);

            if (contribution == null) return BadRequest(new { Message = "Contribution is not found" });
            
            DeleteFile(contribution.contribution_image, "Imgs");

            contribution.contribution_image = await SaveFileAsync(file, "Imgs");
 
            _context.Entry(contribution).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Update image thumbnail successfully" });
        }

        [HttpPut("public-contribution")]
        public async Task<IActionResult> publicContribution(int contribution_id)
        {
            if (contribution_id == null) { return BadRequest(new { Message = "Data is provided is null" }); }

            var contribution = await _context.Contributions.FirstOrDefaultAsync(c => c.contribution_id == contribution_id);

            if (contribution == null) { return BadRequest(new { Message = "Contribution is not found" }); }

            contribution.IsSelected = IsSelected.Selected;

            _context.Entry(contribution).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Contribution is deleted" });
        }

        [HttpGet("get-all-public-contribution")]
        public async Task<IActionResult> getAllPublicContribution()
        {
            var lstPublicContribution = _context.Contributions.Where(c => c.IsSelected == IsSelected.Selected).ToList();

            return Ok(lstPublicContribution);
        }

        [HttpGet("Download-Articles")]
        public async Task<IActionResult> DownloadFiles(string fileNames)
        {
            string[] arrFileNames;

            arrFileNames = fileNames.Split(',');
   
            var zipFileName = "download.zip";

            MemoryStream streamFile = new MemoryStream();

            using (var zipArchive = new ZipArchive(streamFile, ZipArchiveMode.Create, true))
            {
                foreach (var fileName in arrFileNames)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Articles", fileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        var entryName = Path.GetFileName(filePath);
                        zipArchive.CreateEntryFromFile(filePath, entryName);
                    }
                }
            }

            streamFile.Position = 0;

            return File(streamFile, "application/zip", zipFileName);
        }
    }
}
