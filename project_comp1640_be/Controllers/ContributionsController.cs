using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient.Cypher;
using project_comp1640_be.Data;
using project_comp1640_be.Model;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Text;

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

             _context.Contributions.Add(contribution);
            await _context.SaveChangesAsync();

            return Ok(new {Message = "Add article successed"});
        }

        [HttpGet("Get-Article")]
        public async Task<IActionResult> GetArticle(int contribution_id)
        {
            if(contribution_id == null)
            {
                return BadRequest(new {Message = "Data is null" });
            }
            
            var contribution = await _context.Contributions.FindAsync(contribution_id);
            if (contribution == null)
            {
                return NotFound(new {Message = "Couldn't find" });
            }
            return Ok(contribution);
        }
    }
}
