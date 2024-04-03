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
        public async Task<IActionResult> UpdateComment(int id, Marketing_Comments comments)
        {
            var exitComment = await _context.Marketing_Comments.FindAsync(id);
            if (exitComment != null)
            {
                return NotFound(new {Message = "Comment is not found"});
            }
            _context.Entry(exitComment).State = EntityState.Detached;

            if (comments == null)
            {
                return BadRequest(new { Message = "Comment is null" });
            }

            comments.comment_id = id;

            _context.Entry(comments).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Update comment successed" });
        }

        [HttpDelete("delete-comment")]
        public async Task<IActionResult> deleteComment(int comment_id)
        {
            if (comment_id == 0 || comment_id == null) { BadRequest(new {Message = "Comment ID is null"}); }

            var comment = await _context.Marketing_Comments.FirstOrDefaultAsync(c => c.comment_id ==  comment_id);

            if (comment == null) { BadRequest(new { Message = "Comment is not found" }); }

            _context.Marketing_Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Delete Comment successfully" });
        }
    }
}
