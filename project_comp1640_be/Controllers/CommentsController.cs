using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_comp1640_be.Data;
using project_comp1640_be.Model;

namespace project_comp1640_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context) => context = context;

        [HttpGet]
        public async Task<IActionResult> getAllComments()
        {
            var lstComment = _context.Marketing_Comments
                .Include(c => c.users)
                .Include(c => c.contributions)
                .Select(c => new
                {
                    comment_id = c.comment_id,
                    comment_title = c.comment_content,
                    comment_date = c.comment_date,
                    comment_user_id = c.users.user_id,
                    comment_username = c.users.user_username,
                    comment_contribution_id = c.contributions.contribution_id,
                    comment_contribution = c.contributions.contribution_title
                })
                .ToList();

            if (lstComment == null)
                return BadRequest();

            return Ok(lstComment);
        }


        [HttpPost]
        public async Task<IActionResult> addComment(Marketing_Comments comment)
        {
            if(comment == null)
                return BadRequest();

            _context.Marketing_Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok(new {Messasge = "Add Comment Succeed"});
        }
    }
}
