using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using project_comp1640_be.Data;
using project_comp1640_be.Model;

namespace project_comp1640_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> getAllRole()
        {
            var roles = _context.Roles.ToList();

            return Ok(roles);
        }

        [HttpGet("role_name")]
        public async Task<IActionResult> getIdRole(string role_name)
        {
            var obj = _context.Roles.FirstOrDefault(c => c.role_name == role_name);

            if (obj == null)
                return NotFound(new { Message = "Role is not found!" });

            return Ok(obj.role_id);
        }

        [HttpPost("add-role")]
        public async Task<IActionResult> addRole(Roles roles)
        {
            if(roles == null)
                return BadRequest(new {Message = "Add Role Failed"});

            _context.Roles.Add(roles);
            await _context.SaveChangesAsync();

            return Ok(new {message = "Add Role Succeed"});
        }
    }
}
