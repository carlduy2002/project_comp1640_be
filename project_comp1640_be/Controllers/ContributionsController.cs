using Aspose.Words.Saving;
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
using project_comp1640_be.Model.Dto;
using Neo4jClient.DataAnnotations.Cypher.Functions;
using Aspose.Words.Bibliography;

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

        //get all article of student
        [HttpGet("username")]
        public async Task<IActionResult> getArticleOfStudent(string username)
        {
            if (username == null)
                return BadRequest(new { Message = "Username is null" });

            var userId = _context.Users.Where(u => u.user_username.Equals(username)).Select(u => u.user_id).FirstOrDefault();

            if (userId == null)
                return NotFound(new { Message = "User is not found" });

            var lstArticle = _context.Contributions.Where(c => c.contribution_user_id.Equals(userId)).ToList();

            return Ok(lstArticle);

        }

        //get article by id
        [HttpGet("contribution_id")]
        public async Task<IActionResult> getArticleById(int contribution_id)
        {
            if (contribution_id == 0)
                return BadRequest();

            var article = _context.Contributions
                .Where(c => c.contribution_id.Equals(contribution_id))
                .ToList();

            if (article == null)
                return NotFound(new { Message = "Article is not found" });

            return Ok(article);
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
        private void SendEmail(string email, int contributionId)
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var emailToken = Convert.ToBase64String(tokenBytes);

            string from = _configuration["EmailSettings:From"];
            var emailBody = EmailBody.AddNewArticleEmailStringBody(contributionId);
            var emailModel = new EmailModel(email, "New ariticle posted!!", emailBody);
            _emailService.SendEmail(emailModel);
        }

        [HttpPost("Add-New-Article")]
        public async Task<IActionResult> AddNewArticle()
        {
            try
            {
                Contributions con = new Contributions();

                var httpRequest = Request.Form;

                var title = Request.Form["title"];
                if (title.ToString().Trim().Equals(""))
                {
                    return BadRequest(new { Message = "Title is empty" });
                }

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
                var academicyear = _context.Academic_Years.Where(academicYear => academicYear.academic_year_ClosureDate.Year == date.Year).Select(a => a.academic_year_id).FirstOrDefault();
                con.contribution_academic_years_id = academicyear;

                var user = _context.Users.Where(u => u.user_username.Equals(username) && u.user_status == user_status.Unlock).FirstOrDefault();
                if (user == null)
                {
                    return BadRequest(new { Message = "User not found" });
                }
                var userId = user.user_id;
                con.contribution_user_id = userId;

                con.contribution_title = title;
                con.contribution_submition_date = date;
                con.IsEnabled = IsEnabled.Enabled.ToString();
                con.IsSelected = IsSelected.Pending.ToString();
                con.IsView = IsView.Unview.ToString();
                con.IsPublic = IsPublic.Private.ToString();

                _context.Contributions.Add(con);
                await _context.SaveChangesAsync();

                // get user faculty
                //var userFaculty = _context.Users.Where(u => u.user_username.Equals(username)).Select(u => u.user_id).FirstOrDefault();

                // fine faculty manager and send email
                var maketingCondinatorUser = _context.Users.Where(u => u.user_faculty_id == user.user_faculty_id && u.role.role_name.Equals("Coordinator")).FirstOrDefault();
                var maketingCondinatorEmail = maketingCondinatorUser.user_email;
                SendEmail(maketingCondinatorEmail, con.contribution_id);

                return Ok(new { Message = "Add article succeeded" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Upload File Not Succeed - " + " Error: " + ex });
            }
        }

        [HttpPost("Update-Article")]
        public async Task<IActionResult> UpdateArticle()
        {
            try
            {
                var httpRequest = HttpContext.Request.Form;

                var title = httpRequest["title"];

                if (title.ToString().Trim().Equals(""))
                {
                    return BadRequest(new { Message = "Title is empty" });
                }

                var contributionID = httpRequest["contribution_id"];

                var username = httpRequest["username"];

                var submitDate = httpRequest["submitDate"];

                var article = httpRequest.Files["uploadFile"];

                var thumbnailImg = httpRequest.Files["uploadImage"];

                var currentDate = DateTime.UtcNow;

                var submitDatetime = _context.Contributions
                    .Where(c => c.contribution_id.Equals(int.Parse(contributionID)))
                    .Select(c => c.contribution_submition_date)
                    .FirstOrDefault();

                var getContribution = _context.Contributions
                    .Where(c => c.contribution_id.Equals(int.Parse(contributionID)))
                    .FirstOrDefault();

                var getAcademicYear = _context.Academic_Years
                    .Where(a => a.academic_year_id == getContribution.contribution_academic_years_id)
                    .Select(a => a.academic_year_FinalClosureDate)
                    .FirstOrDefault();


                var test = DateTime.Parse("2024-03-1 07:00:00.0000000");
                //submitDatetime.AddDays(14)

                if (getContribution.IsSelected.Equals(IsSelected.Unselected.ToString()) || getContribution.IsSelected.Equals(IsSelected.Pending.ToString()))
                {
                    if (currentDate < test || currentDate < getAcademicYear)
                    {
                        if (article == null && thumbnailImg == null)
                        {
                            var con = _context.Contributions
                            .Where(c => c.contribution_id == int.Parse(contributionID))
                            .Select(c => new Contributions
                            {
                                contribution_id = c.contribution_id,
                                contribution_title = c.contribution_title,
                                contribution_content = c.contribution_content,
                                contribution_image = c.contribution_image,
                                contribution_submition_date = c.contribution_submition_date,
                                contribution_academic_years_id = c.contribution_academic_years_id,
                                contribution_user_id = c.contribution_user_id
                            })
                            .FirstOrDefault();

                            var user = _context.Users.Where(u => u.user_username.Equals(username)).FirstOrDefault();
                            var userId = user.user_id;
                            con.contribution_user_id = userId;

                            con.contribution_id = int.Parse(contributionID);
                            con.contribution_title = title;
                            con.IsSelected = IsSelected.Unselected.ToString();
                            con.IsEnabled = getContribution.IsEnabled;
                            con.IsView = getContribution.IsView;
                            con.IsPublic = getContribution.IsPublic;
                            //con.contribution_submition_date = DateTime.Parse(submitDate);

                            //_context.Contributions.Entry(con).State = EntityState.Modified;
                            _context.Entry(getContribution).CurrentValues.SetValues(con);

                            await _context.SaveChangesAsync();

                            return Ok(new { Message = "Update article succeeded" });
                        }
                        else if (article != null && thumbnailImg == null)
                        {
                            var con = _context.Contributions
                            .Where(c => c.contribution_id == int.Parse(contributionID))
                            .Select(c => new Contributions
                            {
                                contribution_id = c.contribution_id,
                                contribution_title = c.contribution_title,
                                contribution_content = c.contribution_content,
                                contribution_image = c.contribution_image,
                                contribution_submition_date = c.contribution_submition_date,
                                contribution_academic_years_id = c.contribution_academic_years_id,
                                contribution_user_id = c.contribution_user_id
                            })
                            .FirstOrDefault();

                            if (IsWordFile(article.FileName))
                            {
                                // upload article
                                con.contribution_content = await SaveFileAsync(article, "Articles");
                            }
                            else
                            {
                                return BadRequest(new { Message = "File Format Not Supported" });
                            }

                            var user = _context.Users.Where(u => u.user_username.Equals(username)).FirstOrDefault();
                            var userId = user.user_id;
                            con.contribution_user_id = userId;

                            con.contribution_id = int.Parse(contributionID);
                            con.contribution_title = title;
                            con.contribution_content = article.FileName;
                            con.IsSelected = IsSelected.Unselected.ToString();
                            con.IsEnabled = getContribution.IsEnabled;
                            con.IsView = getContribution.IsView;
                            con.IsPublic = getContribution.IsPublic;
                            //con.contribution_submition_date = DateTime.Parse(submitDate);

                            //_context.Entry(con).State = EntityState.Modified;
                            _context.Entry(getContribution).CurrentValues.SetValues(con);
                            await _context.SaveChangesAsync();

                            // get user faculty
                            //var userFacultyID = _context.Users.Where(u => u.user_username.Equals(username)).Select(u => u.user_id).FirstOrDefault();

                            // fine faculty manager and send email
                            var maketingCondinatorUser = _context.Users
                                .Where(u => u.user_faculty_id == user.user_faculty_id && u.role.role_name.Equals("Coordinator")).FirstOrDefault();


                            var maketingCondinatorEmail = maketingCondinatorUser.user_email;
                            SendEmail(maketingCondinatorEmail, con.contribution_id);

                            return Ok(new { Message = "Update article succeeded" });
                        }
                        else if (article == null && thumbnailImg != null)
                        {
                            var con = _context.Contributions
                            .Where(c => c.contribution_id == int.Parse(contributionID))
                            .Select(c => new Contributions
                            {
                                contribution_id = c.contribution_id,
                                contribution_title = c.contribution_title,
                                contribution_content = c.contribution_content,
                                contribution_image = c.contribution_image,
                                contribution_submition_date = c.contribution_submition_date,
                                contribution_academic_years_id = c.contribution_academic_years_id,
                                contribution_user_id = c.contribution_user_id
                            })
                            .FirstOrDefault();

                            if (IsImageFile(thumbnailImg.FileName))
                            {
                                // upload tumbnail img
                                con.contribution_image = await SaveFileAsync(thumbnailImg, "Imgs");
                            }
                            else
                            {
                                return BadRequest(new { Message = "File Format Not Supported" });
                            }

                            var user = _context.Users.Where(u => u.user_username.Equals(username)).FirstOrDefault();
                            var userId = user.user_id;
                            con.contribution_user_id = userId;

                            con.contribution_id = int.Parse(contributionID);
                            con.contribution_title = title;
                            con.contribution_image = thumbnailImg.FileName;
                            con.IsSelected = IsSelected.Unselected.ToString();
                            con.IsEnabled = getContribution.IsEnabled;
                            con.IsView = getContribution.IsView;
                            con.IsPublic = getContribution.IsPublic;
                            //con.contribution_submition_date = DateTime.Parse(submitDate);

                            //_context.Entry(con).State = EntityState.Modified;
                            _context.Entry(getContribution).CurrentValues.SetValues(con);
                            await _context.SaveChangesAsync();

                            // get user faculty
                            //var userFacultyID = _context.Users.Where(u => u.user_username.Equals(username)).Select(u => u.faculties.faculty_id).FirstOrDefault();

                            // fine faculty manager and send email
                            var maketingCondinatorUser = _context.Users
                                .Where(u => u.user_faculty_id == user.user_faculty_id && u.role.role_name.Equals("Coordinator")).FirstOrDefault();


                            var maketingCondinatorEmail = maketingCondinatorUser.user_email;
                            SendEmail(maketingCondinatorEmail, con.contribution_id);

                            return Ok(new { Message = "Update article succeeded" });

                        }
                        else
                        {
                            var con = _context.Contributions
                            .Where(c => c.contribution_id == int.Parse(contributionID))
                            .Select(c => new Contributions
                            {
                                contribution_id = c.contribution_id,
                                contribution_title = c.contribution_title,
                                contribution_content = c.contribution_content,
                                contribution_image = c.contribution_image,
                                contribution_submition_date = c.contribution_submition_date,
                                contribution_academic_years_id = c.contribution_academic_years_id,
                                contribution_user_id = c.contribution_user_id
                            })
                            .FirstOrDefault();

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

                            var user = _context.Users.Where(u => u.user_username.Equals(username)).FirstOrDefault();
                            var userId = user.user_id;
                            con.contribution_user_id = userId;

                            con.contribution_id = int.Parse(contributionID);
                            con.contribution_title = title;
                            con.contribution_content = article.FileName;
                            con.contribution_image = thumbnailImg.FileName;
                            con.IsSelected = IsSelected.Unselected.ToString();
                            con.IsEnabled = getContribution.IsEnabled;
                            con.IsView = getContribution.IsView;
                            con.IsPublic = getContribution.IsPublic;
                            //con.contribution_submition_date = DateTime.Parse(submitDate);

                            _context.Entry(getContribution).CurrentValues.SetValues(con);
                            await _context.SaveChangesAsync();

                            // get user faculty
                            //var userFacultyID = _context.Users.Where(u => u.user_username.Equals(username)).Select(u => u.faculties.faculty_id).FirstOrDefault();

                            // fine faculty manager and send email
                            var maketingCondinatorUser = _context.Users
                                .Where(u => u.user_faculty_id == user.user_faculty_id && u.role.role_name.Equals("Coordinator")).FirstOrDefault();


                            var maketingCondinatorEmail = maketingCondinatorUser.user_email;
                            SendEmail(maketingCondinatorEmail, con.contribution_id);

                            return Ok(new { Message = "Update article succeeded" });
                        }

                        
                    }
                    else
                    {
                        return BadRequest(new { Message = "This Contribution Expried Update!" });
                    }
                }
                else 
                {
                    return BadRequest(new { Message = "This Contribution is Approved" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Upload File Not Succeed - " + " Error: " + ex });
            }
        }

        //load file word to html
        private async Task<String> LoadWordFile(string fileName)
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

                string htmlContent = new StreamReader(stream, Encoding.UTF8).ReadToEnd();


                //htmlContent = Regex.Replace(htmlContent, @"(Evaluation Only\. Created with Aspose\.Words\. Copyright \d{4}-\d{4} Aspose Pty Ltd\.|Created with an evaluation copy of Aspose\.Words\. To discover the full versions of our APIs please visit: https://products\.aspose\.com/words/)", string.Empty);

                //htmlContent = htmlContent.Replace("This document was truncated here because it was created in the Evaluation Mode.", "");

                return htmlContent;
            }
            return "File not found";
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

            var wordFile = LoadWordFile(contribution.contribution_content);

            contribution.contribution_content = await wordFile;
            return Ok(new { result = contribution });
        }

        [HttpGet("Get-Article-Of-Student")]
        public async Task<IActionResult> GetArticleByUsername(string username, int contribution_id)
        {
            if (username == null)
            {
                return BadRequest(new { Message = "Data is null" });
            }

            //var userID = _context.Users.Where(u => u.user_username.Equals(username)).Select(u => u.user_id).FirstOrDefault();

            var contribution = _context.Contributions.Where(c => c.users.user_username == username && c.contribution_id == contribution_id).FirstOrDefault();

            if (contribution == null)
            {
                return NotFound(new { Message = "Couldn't find" });
            }

            var wordFile = LoadWordFile(contribution.contribution_content);

            contribution.contribution_content = await wordFile;

            return Ok(new { result = contribution });
        }

        [HttpGet("Get-All-Article-Of-Student")]
        public async Task<IActionResult> GetAllArticleByUsername(string username, int contribution_id)
        {
            if (username == null)
            {
                return BadRequest(new { Message = "Data is null" });
            }

            var userID = _context.Users.Where(u => u.user_username.Equals(username)).Select(u => u.user_id).FirstOrDefault();

            var contribution = _context.Contributions.Where(c => c.contribution_user_id == userID).ToList();

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

        [HttpGet("Get-All-Articles-Selected")]
        public async Task<IActionResult> GetAllArticlesSelected()
        {
            var contributions = await _context.Contributions
                .Include(c => c.academic_years)
                .Where(c => c.IsSelected.Equals(IsSelected.Selected.ToString()))
                .Select(c => new
                {
                    contribution_image = c.contribution_image,
                    contribution_id = c.contribution_id,
                    contribution_title = c.contribution_title,
                    contribution_submition_date = c.contribution_submition_date,
                    contribution_academic_years_id = c.academic_years.academic_year_title,
                    isPublic = c.IsPublic.ToString(),
                    isSelected = c.IsSelected.ToString(),
                    isEnabled = c.IsEnabled.ToString(),
                    isView = c.IsView.ToString()
                })
                .ToListAsync();
            return Ok(contributions);
        }

        [HttpGet("Get-All-Articles-Public")]
        public async Task<IActionResult> GetAllArticlesApprove()
        {
            var contributions = await _context.Contributions
            .Where(c => c.IsPublic.Equals(IsPublic.Public.ToString()))
                .ToListAsync();
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

        [HttpDelete("delete-contribution")]
        public async Task<IActionResult> deleteContribution(int contribution_id)
        {
            if (contribution_id == null) { return BadRequest(new { Message = "Data is provided is null" }); }

            var contribution = await _context.Contributions.FirstOrDefaultAsync(c => c.contribution_id == contribution_id);

            if (contribution == null) { return BadRequest(new { Message = "Contribution is not found" }); }

            var comments = await _context.Marketing_Comments.Where(m => m.comment_contribution_id == contribution_id).ToListAsync();

            DeleteFile(contribution.contribution_content, "Articles");
            DeleteFile(contribution.contribution_image, "Imgs");

            _context.Marketing_Comments.RemoveRange(comments);
            await _context.SaveChangesAsync();

            _context.Contributions.Remove(contribution);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Contribution is deleted" });
        }

        [HttpPut("View")]
        public async Task<IActionResult> View(int contribution_id)
        {
            var contribution = _context.Contributions.Where(c => c.contribution_id == contribution_id).FirstOrDefault();

            if (contribution == null)
                return BadRequest(new { Message = "Cannot change status view" });

            contribution.IsView = IsView.View.ToString();

            _context.Entry(contribution).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("Approve")]
        public async Task<IActionResult> Approve(int contribution_id)
        {
            var contribution = _context.Contributions.Where(c => c.contribution_id == contribution_id).FirstOrDefault();

            var currentDate = DateTime.UtcNow;

            var academicYearID = _context.Contributions
                .Where(c => c.contribution_id == contribution_id)
                .Select(c => c.contribution_academic_years_id)
                .FirstOrDefault();

            var getAcademicYear = _context.Academic_Years
                .Where(a => a.academic_year_id == academicYearID)
                .Select(a => a.academic_year_FinalClosureDate)
                .FirstOrDefault();

            if (contribution == null)
                return BadRequest(new { Message = "Cannot change status approve" });


            //var test = DateTime.Parse("2025-01-01 07:00:00.0000000");

            if (currentDate < getAcademicYear)
            {
                contribution.IsSelected = IsSelected.Selected.ToString();

                _context.Entry(contribution).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new {Message = "Approve Succeed"});
            }
            else
            {
                return BadRequest(new { Message = "This Contribution Expried Approve!" });
            }
        }


        [HttpPut("Reject")]
        public async Task<IActionResult> Reject(int contribution_id)
        {
            var contribution = _context.Contributions.Where(c => c.contribution_id == contribution_id).FirstOrDefault();

            var currentDate = DateTime.UtcNow;

            var academicYearID = _context.Contributions
               .Where(c => c.contribution_id == contribution_id)
               .Select(c => c.contribution_academic_years_id)
               .FirstOrDefault();

            var getAcademicYear = _context.Academic_Years
                .Where(a => a.academic_year_id == academicYearID)
                .Select(a => a.academic_year_FinalClosureDate)
                .FirstOrDefault();

            if (contribution == null)
                return BadRequest(new { Message = "Cannot change status approve" });

            //var test = DateTime.Parse("2025-01-01 07:00:00.0000000");

            if (currentDate < getAcademicYear)
            {
                contribution.IsSelected = IsSelected.Unselected.ToString();

                _context.Entry(contribution).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new {Message = "Reject Succeed"});
            }
            else
            {
                return BadRequest(new { Message = "This Contribution Expried Reject!" });
            }
        }

        [HttpPut("Public")]
        public async Task<IActionResult> Public(int contribution_id)
        {
            var contribution = _context.Contributions.Where(c => c.contribution_id == contribution_id).FirstOrDefault();

            if (contribution == null)
                return BadRequest(new { Message = "Cannot change status approve" });

            contribution.IsPublic = IsPublic.Public.ToString();

            _context.Entry(contribution).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Public successful!!" });
        }

        [HttpPut("Private")]
        public async Task<IActionResult> Private(int contribution_id)
        {
            var contribution = _context.Contributions.Where(c => c.contribution_id == contribution_id).FirstOrDefault();

            if (contribution == null)
                return BadRequest(new { Message = "Cannot change status approve" });

            contribution.IsPublic = IsPublic.Private.ToString();
            contribution.IsSelected = IsSelected.Selected.ToString();

            _context.Entry(contribution).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Private successful!!" });
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

        [HttpGet("Download-One-Article")]
        public async Task<IActionResult> DownloadFile(int contribution_id)
        {
            var contribution = await _context.Contributions.FirstOrDefaultAsync(c => c.contribution_id == contribution_id && c.IsSelected.Equals(IsSelected.Selected.ToString()));

            if (contribution == null) { return BadRequest(new { Message = "Contribution is not found" }); }

            string fileName = contribution.contribution_content;

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Articles", fileName);

            var zipFileName = "download.zip";

            MemoryStream streamFile = new MemoryStream();

            using (var zipArchive = new ZipArchive(streamFile, ZipArchiveMode.Create, true))
            {
                if (System.IO.File.Exists(filePath))
                {
                    var entryName = Path.GetFileName(filePath);
                    zipArchive.CreateEntryFromFile(filePath, entryName);
                }
                else
                {
                    return BadRequest(new { Message = "File does not exist" });
                }
            }

            streamFile.Position = 0;

            return File(streamFile, "application/zip", zipFileName);
        }

        [HttpGet("Download-Many-Article")]
        public async Task<IActionResult> DownloadFiles(int faculty_id, int acdemic_year_id)
        {
            var contributions = new List<Contributions>();

            if (faculty_id != 0 && acdemic_year_id != 0)
            {
                contributions = await _context.Contributions
                    .Where(c => c.users.user_faculty_id == faculty_id
                             && c.contribution_academic_years_id == acdemic_year_id
                             && c.IsSelected.Equals(IsSelected.Selected.ToString()))
                    .ToListAsync();
            }
            else if (faculty_id != 0 && acdemic_year_id == 0)
            {
                contributions = await _context.Contributions
                    .Where(c => c.users.user_faculty_id == faculty_id
                             //&& c.academic_years.academic_year_ClosureDate >= DateTime.Now
                             && c.IsSelected.Equals(IsSelected.Selected.ToString()))
                    .ToListAsync();
            }
            else if (faculty_id == 0 && acdemic_year_id != 0)
            {
                contributions = await _context.Contributions
                    .Where(c => c.contribution_academic_years_id.Equals(acdemic_year_id)
                             && c.IsSelected.Equals(IsSelected.Selected.ToString()))
                    .ToListAsync();
            }   
            else
            {
                contributions = await _context.Contributions
                    .Where(c => 
                           //c.academic_years.academic_year_ClosureDate >= DateTime.Now && 
                           c.IsSelected.Equals(IsSelected.Selected.ToString()))
                    .ToListAsync();
            }

            if (contributions == null) { return BadRequest(new { Message = "Can not found Contribution" }); }

            List<string> FileNamesList = new List<string>();
            foreach (var contribution in contributions)
            {
                FileNamesList.Add(contribution.contribution_content.ToString());
            }

            var zipFileName = "download.zip";

            MemoryStream streamFile = new MemoryStream();

            using (var zipArchive = new ZipArchive(streamFile, ZipArchiveMode.Create, true))
            {
                foreach (var fileName in FileNamesList)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Articles", fileName.ToString());
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

        [HttpGet("GetContributionByFaculty")]
        public async Task<IActionResult> GetContributionByFaculty(string username)
        {
            var user = _context.Users
                .Where(u => u.user_username.Equals(username))
                .Select(u => u.user_faculty_id)
                .FirstOrDefault();

            var lstUser = _context.Users
                .Include(l => l.role)
                .Where(l => l.user_faculty_id.Equals(user) && l.role.role_name.Equals("Student"))
                .Select(l => l.user_id)
                .ToList();

            List<ContributionDTO> lstContribution = new List<ContributionDTO>();

            foreach (var i in lstUser)
            {
                var contribution = _context.Contributions
                    .Include(c => c.users)
                    .Include(c => c.academic_years)
                    .Where(c => c.contribution_user_id.Equals(i) && c.IsEnabled.Equals(IsEnabled.Enabled.ToString()))
                    .Select(c => new ContributionDTO
                    {
                        contribution_id = c.contribution_id,
                        user_id = c.users.user_id,
                        username = c.users.user_username,
                        contribution_image = c.contribution_image,
                        contribution_title = c.contribution_title,
                        contribution_submition_date = c.contribution_submition_date,
                        final_clouser_date = c.academic_years.academic_year_FinalClosureDate,
                        isView = c.IsView.ToString(),
                        isPublic = c.IsPublic.ToString(),
                        isSelected = c.IsSelected.ToString(),
                        isEnabled = c.IsEnabled.ToString()
                    })
                    .ToList();
                foreach (var a in contribution)
                {
                    lstContribution.Add(a);
                }
            }

            if (lstContribution.Count == 0)
                return BadRequest(new { Message = "Don't have any contribution in this faculty" });

            return Ok(lstContribution);
        }

        [HttpGet("GetContributionOfFaculty")]
        public async Task<IActionResult> GetContributionOfFaculty()
        {
            List<object> contributionImg = new List<object>();

            var faculties = _context.Faculties.Where(f => !f.faculty_name.Equals("None")).ToList();

            foreach (var faculty in faculties)
            {
                var f = _context.Contributions
                    .Select(c => new {
                    id = faculty.faculty_id,
                    name = faculty.faculty_name,
                    img = _context.Contributions.Where(c => c.users.user_faculty_id == faculty.faculty_id).Select(c => c.contribution_image).FirstOrDefault()
            }).FirstOrDefault();
                contributionImg.Add(f);
            }
            return Ok(contributionImg);
        }
        [HttpGet("GetContributionByFacultyId")]
        public async Task<IActionResult> GetContributionByFacultyId(int facultyId)
        {
            var contributions = _context.Contributions.Where(c => c.users.user_faculty_id == facultyId && c.IsPublic == IsPublic.Public.ToString()).ToList();

            if(contributions.Count <= 0)
            {
                return BadRequest(new { Message = "Not found" });
            }
            return Ok(contributions);
        }
        [HttpGet("search")]
        public async Task<IActionResult> search(int academic_id, int faculty_id)
        {
            List<object> searchContribution = new List<object>();

            if (academic_id != 0 && faculty_id != 0)
            {
                var contributions = await _context.Contributions
                .Include(c => c.academic_years)
                .Include(c => c.users)
                .Where(c => c.IsSelected.Equals(IsSelected.Selected.ToString()) && c.contribution_academic_years_id == academic_id
                        && c.users.user_faculty_id == faculty_id)
                .Select(c => new
                {
                    contribution_image = c.contribution_image,
                    contribution_id = c.contribution_id,
                    contribution_title = c.contribution_title,
                    contribution_submition_date = c.contribution_submition_date,
                    contribution_academic_years_id = c.academic_years.academic_year_title,
                    isPublic = c.IsPublic.ToString(),
                    isSelected = c.IsSelected.ToString(),
                    isEnabled = c.IsEnabled.ToString(),
                    isView = c.IsView.ToString()
                })
                .ToListAsync();

                return Ok(contributions);
            }
            else if (academic_id != 0 && faculty_id == 0)
            {
                var contributions = await _context.Contributions
                .Include(c => c.academic_years)
                .Include(c => c.users)
                .Where(c => c.IsSelected.Equals(IsSelected.Selected.ToString()) && c.contribution_academic_years_id == academic_id)
                .Select(c => new
                {
                    contribution_image = c.contribution_image,
                    contribution_id = c.contribution_id,
                    contribution_title = c.contribution_title,
                    contribution_submition_date = c.contribution_submition_date,
                    contribution_academic_years_id = c.academic_years.academic_year_title,
                    isPublic = c.IsPublic.ToString(),
                    isSelected = c.IsSelected.ToString(),
                    isEnabled = c.IsEnabled.ToString(),
                    isView = c.IsView.ToString()
                })
                .ToListAsync();

                return Ok(contributions);
            }
            else
            {
                var contributions = await _context.Contributions
                .Include(c => c.academic_years)
                .Include(c => c.users)
                .Where(c => c.IsSelected.Equals(IsSelected.Selected.ToString()) && c.users.user_faculty_id == faculty_id)
                .Select(c => new
                {
                    contribution_image = c.contribution_image,
                    contribution_id = c.contribution_id,
                    contribution_title = c.contribution_title,
                    contribution_submition_date = c.contribution_submition_date,
                    contribution_academic_years_id = c.academic_years.academic_year_title,
                    isPublic = c.IsPublic.ToString(),
                    isSelected = c.IsSelected.ToString(),
                    isEnabled = c.IsEnabled.ToString(),
                    isView = c.IsView.ToString()
                })
                .ToListAsync();

                return Ok(contributions);
            }
        }
    }
}