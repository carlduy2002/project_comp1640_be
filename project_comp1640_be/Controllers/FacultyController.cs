using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_comp1640_be.Data;
using project_comp1640_be.Model;

namespace project_comp1640_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacultyController : ControllerBase
    {
        private ApplicationDbContext _context;

        public FacultyController(ApplicationDbContext context) => _context = context;

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id) {

            var faculty = await _context.Faculties.FindAsync(id);

            if (faculty == null) { 
                return NotFound();
            }

            _context.Faculties.Remove(faculty);
            await _context.SaveChangesAsync();  

            return NoContent();
        }

    }
}
