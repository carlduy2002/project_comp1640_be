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

        [HttpPost("uploadFile")]
        public async Task<IActionResult> uploadFile()
        {
            //var filePaths = new List<string>();

            //var filePath = Path.Combine(Directory.GetCurrentDirectory(), folder, file.FileName);
            //filePaths.Add(filePath);

            //using (var stream = new FileStream(filePath, FileMode.Create))
            //{
            //    file.CopyTo(stream);
            //}

            try
            {
                var httpRequest = Request.Form;
                var postFile = httpRequest.Files[0];
                string fileName = postFile.FileName;
                //var physicalPath = _env.ContentRootPath + "/Articles/" + fileName;
                var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "Articles", fileName);


                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    postFile.CopyTo(stream);
                }

                return new JsonResult(fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Upload File Not Succeed" });
            }
        }

        [HttpPost("Add-New-Article")]
        public async Task<IActionResult> AddNewArticle()
        {

           
            //return Ok(new {Message = "Add Contribution Succeed"});

            try
            {
                var httpRequest = Request.Form;

                var title = Request.Form["title"];
                var username = Request.Form["username"];
                

                var postFile = httpRequest.Files[0];
                string fileName = postFile.FileName;
                var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "Articles", fileName);

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

                var userId = _context.Users.Where(u => u.user_username == username).Select(u=> u.user_id).FirstOrDefault();
                var date = DateTime.Now;

                var academicyear = _context.Academic_Years.Where(a=> a.academic_Year_startClosureDate >= date && a.academic_Year_endClosureDate <= date).Select( a=> a.academic_year_id).FirstOrDefault();

                Contributions con = new Contributions();

                con.contribution_title = title;
                con.contribution_content = fileName;
                con.contribution_image = "kiet";
                con.contribution_user_id = userId;
                con.contribution_academic_years_id = academicyear;
                con.IsEnabled = IsEnabled.Enabled;
                con.IsSelected = IsSelected.Selected;
                con.IsView = IsView.View;

                _context.Contributions.Add(con);
                await _context.SaveChangesAsync();


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

        // function to download many file
        [HttpGet("dl")]
        public FileResult downloadFiles(string fileNames)
        {
            var zipFileName = "download.zip";

            MemoryStream streamFile = new MemoryStream();

            string[] arrFileNames = fileNames.Split(',');

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
