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

            var htmlContent = LoadFile(contribution.contribution_content);

            return Ok(contribution + htmlContent);
        }

        public string LoadFile(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Articles", fileName);

            if (System.IO.File.Exists(filePath))
            {
                /*Document doc = new Document(filePath);

                // doc.RemoveMacros();

                MemoryStream stream = new MemoryStream();

                HtmlSaveOptions options = new HtmlSaveOptions();
                options.ExportImagesAsBase64 = true;

                doc.Save(stream, options);

                stream.Position = 0;

                string htmlContent = new StreamReader(stream, Encoding.UTF8).ReadToEnd();

                htmlContent = Regex.Replace(htmlContent, @"(Evaluation Only\. Created with Aspose\.Words\. Copyright \d{4}-\d{4} Aspose Pty Ltd\.|Created with an evaluation copy of Aspose\.Words\. To discover the full versions of our APIs please visit: https://products\.aspose\.com/words/)", string.Empty);

                return (htmlContent);*/
            }
            return ("File dose not exit");
        }

    }
}
