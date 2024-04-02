using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using project_comp1640_be.Data;
using project_comp1640_be.Model;

namespace project_comp1640_be.Controllers
{
    [Route("page-view")]
    [ApiController]
    public class Page_ViewsController : ControllerBase
    {
        readonly ApplicationDbContext _context;

        public Page_ViewsController(ApplicationDbContext context) => _context = context;

        [HttpPost("add-page-view")]
        public async Task<IActionResult> addPageView(Page_Views page_Views)
        {
            if (page_Views == null) return BadRequest(new {Message = "Data provided is null"});

            _context.Page_Views.Add(page_Views);
            await _context.SaveChangesAsync();

            return Ok(new {Message = "Add Page View Successfully"});
        }
    }
}
