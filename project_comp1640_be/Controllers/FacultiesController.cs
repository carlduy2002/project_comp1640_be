using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_comp1640_be.Data;
using project_comp1640_be.Model;

namespace project_comp1640_be.Controllers
{
    [Route("faculty")]
    [ApiController]
    public class FacultiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FacultiesController(ApplicationDbContext context) 
        { 
            _context = context;
        }

        [HttpGet("get-faculty")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> getFaculty(int faculty_id)
        {
            if(faculty_id == null) { return BadRequest( new {Message = "Data is provided is null"}); }

            var faculty = await _context.Faculties.FirstOrDefaultAsync(f => f.faculty_id == faculty_id);

            if (faculty == null) { return BadRequest(new { Message = "Faculty is not found" }); }

            return Ok(faculty);
        }

        [HttpPost("update-faculty")]
        public async Task<IActionResult> updateFaculty([FromBody] Faculties faculty) 
        {
            if (faculty == null) { return BadRequest(new { Message = "Data is provided is null" }); }

            var checkFaculty = await _context.Faculties.FirstOrDefaultAsync(f => f.faculty_id == faculty.faculty_id);

            if (checkFaculty == null) { return BadRequest(new { Message = "Faculty is not found" }); }

            checkFaculty.faculty_name = faculty.faculty_name;
            _context.Entry(checkFaculty).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new {Message = "Update faculty successfull"});
        }

        [HttpDelete("Delete-faculty")]
        public async Task<ActionResult> Delete(int id)
        {

            var faculty = await _context.Faculties.FindAsync(id);

            if (faculty == null)
            {
                return BadRequest(new { Message = "Faculty is not found" });
            }

            _context.Faculties.Remove(faculty);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Delete Succeed"
            });
        }

    }
}
