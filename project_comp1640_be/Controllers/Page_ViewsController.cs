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
        public async Task<IActionResult> addPageView(string page_view_name, string browser_name, int total_time_access, string username)
        {
            if (page_view_name == null || browser_name == null || total_time_access == null) 
                return BadRequest(new {Message = "Data provided is null"});

            var getUserID = _context.Users.Where(u => u.user_username == username)
                                            .Select(c => c.user_id).FirstOrDefault();

            Page_Views page_Views = new Page_Views();
            page_Views.page_view_name = page_view_name;
            page_Views.browser_name = browser_name;
            page_Views.time_stamp = DateTime.Now;
            page_Views.total_time_access = total_time_access;
            page_Views.page_view_user_id = getUserID;

            _context.Page_Views.Add(page_Views);
            await _context.SaveChangesAsync();

            return Ok(new {Message = "Add Page View Successfully"});
        }

        [HttpGet("get-all-page-browse-role")]
        public async Task<IActionResult> getAllPageBrowseRole()
        {
            List<Object> lstData = new List<Object>();

            var lstPage = _context.Page_Views.GroupBy(p => p.page_view_name).Select(p => p.Key.ToString()).ToList();
            var lstBrowser = _context.Page_Views.GroupBy(p => p.browser_name).Select(p => p.Key.ToString()).ToList();
            var lstRole = _context.Page_Views.GroupBy(p => p.users.role.role_name).Select(p => p.Key.ToString()).ToList();

            lstData.Add(lstPage);
            lstData.Add(lstBrowser);
            lstData.Add(lstRole);

            return Ok(lstData);
        }
    }
}
