using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using project_comp1640_be.Data;

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
    }
}
