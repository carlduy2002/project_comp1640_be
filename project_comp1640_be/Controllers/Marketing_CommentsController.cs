using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_comp1640_be.Data;
using project_comp1640_be.Model;

namespace project_comp1640_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Marketing_CommentsController : ControllerBase
    {
        private ApplicationDbContext _context;

        public Marketing_CommentsController(ApplicationDbContext context) => _context = context;

        [HttpPut("Update-Comment")]
        public async Task<IActionResult> UpdateComment(Marketing_Comments comments)
        {
            if (comments == null)
            {
                return BadRequest(new { Message = "Comment is null" });
            }

            _context.Entry(comments).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Update comment successed" });
        }
    }
}
