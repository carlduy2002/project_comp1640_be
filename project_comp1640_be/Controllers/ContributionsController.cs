using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using project_comp1640_be.Data;
using project_comp1640_be.Model;
using System.Data.SqlTypes;
using System.IO.Compression;

namespace project_comp1640_be.Controllers
{
    [Route("contribution")]
    [ApiController]
    public class ContributionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ContributionsController(ApplicationDbContext context)
        {
            _context = context;
        }
        //[HttpPost("uploadFile")]
        //public async Task<IActionResult> uploadFile()
        //{
            //var filePaths = new List<string>();

            //var filePath = Path.Combine(Directory.GetCurrentDirectory(), folder, file.FileName);
            //filePaths.Add(filePath);

            //using (var stream = new FileStream(filePath, FileMode.Create))
            //{
            //    file.CopyTo(stream);
            //}

        //    try
        //    {
        //        var httpRequest = Request.Form;
        //        var postFile = httpRequest.Files[0];
        //        string fileName = postFile.FileName;
        //        //var physicalPath = _env.ContentRootPath + "/Articles/" + fileName;
        //        var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "Articles", fileName);


        //        using (var stream = new FileStream(physicalPath, FileMode.Create))
        //        {
        //            postFile.CopyTo(stream);
        //        }

        //        return new JsonResult(fileName);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { Message = "Upload File Not Succeed" });
        //    }
        //}

        [HttpPost("Add-New-Article")]
        public async Task<IActionResult> AddNewArticle([FromBody] dynamic data)
        {

           
            //return Ok(new {Message = "Add Contribution Succeed"});

            try
            {
                //var httpRequest = Request.Form;

                //var title = Request.Form["title"];
                //var username = Request.Form["username"];

                var title = data.title;
                var username = data.fullname;


                //

                //var postFile = httpRequest.Files["uploadFile"];
                var postFile = data.file1;
                string fileName = postFile.FileName;
                var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "Articles", fileName);

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    postFile.CopyTo(stream);
                }

                //

                //var postFileImg = httpRequest.Files["uploadImage"];
                var postFileImg = data.file2;
                string fileNameImg = postFile.FileName;
                var physicalPathImg = Path.Combine(Directory.GetCurrentDirectory(), "Imgs", fileName);

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    postFile.CopyTo(stream);
                }

                //var filePaths = new List<string>();

                //var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Articles", file.FileName);
                //filePaths.Add(filePath);

                //using (var stream = new FileStream(filePath, FileMode.Create))
                //{
                //    file.CopyTo(stream);
                //}

                var date = DateTime.Now;
                var academicyear = _context.Academic_Years.Where(a => a.academic_Year_startClosureDate >= date).Select(a => a.academic_year_id).FirstOrDefault();


                //var userId = _context.Users.Where(u => u.user_username.Equals(username)).Select(u => u.user_id).FirstOrDefault();

                Contributions con = new Contributions();

                con.contribution_title = title;
                con.contribution_content = fileName;
                con.contribution_image = fileNameImg;
                //con.contribution_user_id = userId;
                con.contribution_academic_years_id = academicyear;
                con.IsEnabled = IsEnabled.Enabled;
                con.IsSelected = IsSelected.Unselected;
                con.IsView = IsView.View;

                _context.Contributions.Add(con);
                await _context.SaveChangesAsync();

                // get user faculty
                //var userFaculty = _context.Users.Where(u => u.user_faculty_id.Equals(username)).Select(u => u.user_id).FirstOrDefault();

                // file faculty manager and send email

                return Ok(new { Message = "Add article succeeded" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Upload File Not Succeed" });
            }

            // Upload thumbnail image
            //var image = Request.Form.Files.FirstOrDefault(f => f.Name == "image");
            //if (image != null)
            //{
            //    if (image.Length <= 0)
            //    {
            //        return BadRequest(new { Message = "Image file length less than or equal to 0" });
            //    }

            //    string imageFileType = Path.GetExtension(image.FileName).ToLower().Trim();
            //    if (imageFileType != ".doc" && imageFileType != ".docx")
            //    {
            //        return BadRequest(new { Message = "Image file format not supported. Only .doc and .docx" });
            //    }

            //    uploadFile(image, "Images");
            //}

            //Upload article
            //var article = Request.Form.Files.FirstOrDefault(f => f.Name == "file");

            //var article = Request.Form.Files[0];

            //if (article != null)
            //{
            //    if (article.Length <= 0)
            //    {
            //        return BadRequest(new { Message = "Article file length less than or equal to 0" });
            //    }

            //    string articleFileType = Path.GetExtension(article.FileName).ToLower().Trim();
            //    if (articleFileType != ".doc" && articleFileType != ".docx")
            //    {
            //        return BadRequest(new { Message = "Article file format not supported. Only .doc and .docx" });
            //    }

            //    uploadFile(article, "Articles");
            //    contribution.contribution_content = article.FileName;
            //}
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
    }
}
