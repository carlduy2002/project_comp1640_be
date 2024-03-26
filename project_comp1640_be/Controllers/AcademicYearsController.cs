using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_comp1640_be.Data;
using project_comp1640_be.Model;

namespace project_comp1640_be.Controllers
{
    [Route("academic-years")]
    [ApiController]
    public class AcademicYearsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AcademicYearsController(ApplicationDbContext context) 
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> getAllAcademicYear()
        {
            return Ok(await _context.Academic_Years.ToListAsync());
        }

        [HttpGet("get-academic-year")]
        public async Task<IActionResult> getAcademicYear(int academic_year_id)
        {
            if (academic_year_id == null) { return BadRequest(new { Message = "Data is provided is null" }); }

            var academicYear = await _context.Academic_Years.FirstOrDefaultAsync(a => a.academic_year_id == academic_year_id);

            //if (academicYear != null) { return BadRequest(new { Message = "Academic Year is already exist" }); }

            return Ok(academicYear);

        }

        [HttpPost("add-academic-year")]
        public async Task<IActionResult> addAcademicYears([FromBody] Academic_Years academic_Years)
        {
            if(academic_Years == null) { return BadRequest(new {Message = "Data is provided is null"}); }

            if(academic_Years.academic_year_ClosureDate >= academic_Years.academic_year_FinalClosureDate )
            {
                return BadRequest(new { Message = "Closure Date cannot equal or grater than Final Closure Date" });
            }

            //academic_Years.academic_Year_startClosureDate = DateTime.Parse(academic_Years.academic_Year_startClosureDate.ToString());
           // academic_Years.academic_Year_endClosureDate = DateTime.Parse(academic_Years.academic_Year_endClosureDate.ToString());

            await _context.AddAsync(academic_Years);
            await _context.SaveChangesAsync();

            return Ok(new {Message = "Add new Academic Year successfully"});
        }

        [HttpPost("update-academic-year")]
        public async Task<IActionResult> updateAcademicYear(Academic_Years academicYears)
        {
            if(academicYears == null) { return BadRequest(new { Message = "Data is provided is null" }); }

            var checkAcademicYear = await _context.Academic_Years.FirstOrDefaultAsync(a => a.academic_year_id == academicYears.academic_year_id);
            
            if (checkAcademicYear == null)
            {
                return BadRequest(new { Message = "Academic Year is not found" });
            }

            if(academicYears.academic_year_ClosureDate >= academicYears.academic_year_FinalClosureDate)
            {
                return BadRequest(new { Message = "Start Closure Date cannot equal or grater than End Closure Date" });
            }

            checkAcademicYear.academic_year_title = academicYears.academic_year_title;
            checkAcademicYear.academic_year_ClosureDate = academicYears.academic_year_ClosureDate;
            checkAcademicYear.academic_year_FinalClosureDate = academicYears.academic_year_FinalClosureDate;

            _context.Entry(checkAcademicYear).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Update Academic Year successfully" });
        }

        //duy
        [HttpDelete]
        public async Task<IActionResult> deleteAcademic(int academic_year_id)
        {
            var academic = _context.Academic_Years.Where(a => a.academic_year_id == academic_year_id).FirstOrDefault();

            if (academic == null)
                return BadRequest(new { Message = "Delete Failed" });

            _context.Academic_Years.Remove(academic);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Delete Succeed" });
        }

    }
}
