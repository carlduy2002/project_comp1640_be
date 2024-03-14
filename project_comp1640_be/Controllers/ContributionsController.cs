using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using project_comp1640_be.Data;
using project_comp1640_be.Model;
using System.Data.SqlTypes;

namespace project_comp1640_be.Controllers
{
    [Route("contribution")]
    [ApiController]
    public class ContributionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContributionsController(ApplicationDbContext context)
        {
            _context = context;
        }
    
        private void uploadFile(IFormFile file, string folder)
        {
            var filePaths = new List<string>();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), folder, file.FileName);
            filePaths.Add(filePath);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
        }

        [HttpPost("Add-New-Article")]
        public async Task<IActionResult> AddNewArticle(Contributions contribution)
        {
            // Upload thumbnail image
            var image = Request.Form.Files.FirstOrDefault(f => f.Name == "image");
            if (image != null)
            {
                if (image.Length <= 0)
                {
                    return BadRequest(new { Message = "Image file length less than or equal to 0" });
                }

                string imageFileType = Path.GetExtension(image.FileName).ToLower().Trim();
                if (imageFileType != ".doc" && imageFileType != ".docx")
                {
                    return BadRequest(new { Message = "Image file format not supported. Only .doc and .docx" });
                }

                uploadFile(image, "Images");
            }

            //Upload article
            var article = Request.Form.Files.FirstOrDefault(f => f.Name == "file");

            // var article = Request.Form.Files[0];

            if (article != null)
            {
                if (article.Length <= 0)
                {
                    return BadRequest(new { Message = "Article file length less than or equal to 0" });
                }

                string articleFileType = Path.GetExtension(article.FileName).ToLower().Trim();
                if (articleFileType != ".doc" && articleFileType != ".docx")
                {
                    return BadRequest(new { Message = "Article file format not supported. Only .doc and .docx" });
                }

                uploadFile(article, "Articles");
                contribution.contribution_content = article.FileName;
            }

            _context.Contributions.Add(contribution);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Add article succeeded" });
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





        // chưa làm đây là chat gpt code
        // Code the function to statistical the percentage of contributions by each Faculty for any academic year (Web API)
        [HttpGet("PercentageByFaculty/{academicYearId}")]
        public async Task<IActionResult> GetContributionPercentageByFaculty(int academicYearId)
        {
            try
            {
                // Get contributions for the specified academic year
                var contributions = await _context.Contributions
                    .Where(c => c.contribution_academic_years_id == academicYearId)
                    .ToListAsync();

                // Calculate the total number of contributions
                int totalContributions = contributions.Count;

                // Group contributions by faculty and count the number of contributions for each faculty
                var contributionCountsByFaculty = contributions
                    .GroupBy(c => c.contribution_user_id)
                    .Select(g => new
                    {
                        FacultyId = g.Key,
                        ContributionCount = g.Count()
                    })
                    .ToList();

                // Retrieve the total number of users per faculty
                var facultyUserCounts = await _context.Users
                    .Where(u => u.user_faculty_id != null)
                    .GroupBy(u => u.user_faculty_id)
                    .Select(g => new
                    {
                        FacultyId = g.Key,
                        UserCount = g.Count()
                    })
                    .ToDictionaryAsync(g => g.FacultyId);

                // Calculate the percentage of contributions for each faculty
                var contributionPercentagesByFaculty = new Dictionary<int, double>();
                foreach (var facultyCount in contributionCountsByFaculty)
                {
                    int facultyId = facultyCount.FacultyId;
                    int contributionCount = facultyCount.ContributionCount;
                    int totalUsersInFaculty = facultyUserCounts.ContainsKey(facultyId) ? facultyUserCounts[facultyId].UserCount : 0;
                    double percentage = totalUsersInFaculty > 0 ? ((double)contributionCount / totalUsersInFaculty) * 100 : 0;
                    contributionPercentagesByFaculty.Add(facultyId, percentage);
                }

                return Ok(contributionPercentagesByFaculty);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }
        }
    }
}
