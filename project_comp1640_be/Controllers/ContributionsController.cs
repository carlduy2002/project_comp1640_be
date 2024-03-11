using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient.Cypher;
using project_comp1640_be.Data;
using project_comp1640_be.Model;

namespace project_comp1640_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContributionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContributionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private void uploadFile(IFormFile file)
        {
            var filePaths = new List<string>();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Articles", file.FileName);
            filePaths.Add(filePath);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
        }

        [HttpPost("Add-New-Article")]
        public async Task<IActionResult> AddNewArticle([FromForm] Contributions contribution, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is not provided or empty.");
            }

            uploadFile(file);

            return Ok(new {Mesage = "Add article successed"});
        }
    }
}
