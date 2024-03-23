﻿using Microsoft.AspNetCore.Http;
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

        public CommentsController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> getAllCommentsByID(int comment_contribution_id)
        {
            var lstComment = _context.Marketing_Comments
                .Include(c => c.users)
                .Include(c => c.contributions)
                .Where(c => c.comment_contribution_id.Equals(comment_contribution_id))
                .OrderBy(c => c.comment_date)
                .Select(c => new
                {
                    comment_id = c.comment_id,
                    comment_content = c.comment_content,
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
        public async Task<IActionResult> addComment(string content, string username, int contribution_id)
        {
            var user_id = _context.Users
                .Where(u => u.user_username.Equals(username))
                .Select(u => u.user_id)
                .FirstOrDefault();

            if(user_id == null)
                return BadRequest();

            Marketing_Comments marketing_Comments = new Marketing_Comments();

            marketing_Comments.comment_content = content;
            marketing_Comments.comment_date = DateTime.Now;
            marketing_Comments.comment_user_id = user_id;
            marketing_Comments.comment_contribution_id = contribution_id;

            _context.Marketing_Comments.Add(marketing_Comments);
            await _context.SaveChangesAsync();

            return Ok(new {Messasge = "Add Comment Succeed"});
        }

        [HttpDelete]
        public async Task<IActionResult> deleteComment(int comment_id)
        {
            var comment = _context.Marketing_Comments.Where(c => c.comment_id == comment_id).FirstOrDefault();

            if (comment == null)
                return BadRequest(new { Message = "Delete Failed" });

            _context.Marketing_Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}
