using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using project_comp1640_be.Data;
using project_comp1640_be.Model;
using System.Data.SqlTypes;
using System.IO;
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



        [HttpPost("Add-New-Article")]
        public async Task<IActionResult> AddNewArticle()
        {
            try
            {
                Contributions con = new Contributions();

                var httpRequest = Request.Form;

                var title = Request.Form["title"];
                var username = Request.Form["username"];

                // upload article
                var article = httpRequest.Files["uploadFile"];
                string fileType = Path.GetExtension(article.FileName).ToLower().Trim();

                con.contribution_content = await SaveFileAsync(article, "Articles");

                // upload tumbnail img
                var thumbnailImg = httpRequest.Files["uploadImage"];
                con.contribution_image = await SaveFileAsync(thumbnailImg, "Imgs");

                // add academic year
                var date = DateTime.Now;
                var academicyear = _context.Academic_Years.Where(a => a.academic_Year_startClosureDate >= date).Select(a => a.academic_year_id).FirstOrDefault();
                con.contribution_academic_years_id = academicyear;

                var userId = _context.Users.Where(u => u.user_username.Equals(username)).Select(u => u.user_id).FirstOrDefault();
                con.contribution_user_id = userId;

                con.contribution_title = title;
                // con.contribution_content = fileName;
                // con.contribution_image = fileNameImg;
                con.contribution_submition_date = date;
                con.IsEnabled = IsEnabled.Enabled;
                con.IsSelected = IsSelected.Unselected;
                con.IsView = IsView.Unview;

                _context.Contributions.Add(con);
                await _context.SaveChangesAsync();

                // get user faculty
                var userFaculty = _context.Users.Where(u => u.user_faculty_id.Equals(username)).Select(u => u.user_id).FirstOrDefault();

                // file faculty manager and send email

                return Ok(new { Message = "Add article succeeded" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Upload File Not Succeed - " + " Error: " + ex});
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
    }
}
