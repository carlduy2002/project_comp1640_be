using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_comp1640_be.Data;
using project_comp1640_be.Model;

namespace project_comp1640_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacultiesController : ControllerBase
    {
        readonly ApplicationDbContext _context;

        public FacultiesController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> getAllFaculty()
        {
            var roles = _context.Faculties.ToList();

            return Ok(roles);
        }

        [HttpGet("faculty_name")]
        public async Task<IActionResult> getIdFaculty(string faculty_name)
        {
            var obj = _context.Faculties.FirstOrDefault(c => c.faculty_name == faculty_name);

            if (obj == null)
                return NotFound(new { Message = "Faculty is not found!" });

            return Ok(obj.faculty_id);
        }

        [HttpPost]
        public async Task<IActionResult> addFaculty(Faculties faculties)
        {
            if(faculties == null)
                return BadRequest(new {Message = "Data to add is null"});

            if (await checkExistFaculty(faculties.faculty_name))
                return BadRequest(new { Message = "Faculty already exist" });

            await _context.Faculties.AddAsync(faculties);
            await _context.SaveChangesAsync();

            return Ok(new {Message = "Add Faculty Succeed"});
        }
        private Task<bool> checkExistFaculty(string faculty_name)
        {
            return _context.Faculties.AnyAsync(f => f.faculty_name == faculty_name);
        }

    }
}
