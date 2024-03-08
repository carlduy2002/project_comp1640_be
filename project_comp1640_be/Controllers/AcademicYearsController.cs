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

        [HttpGet("get-academic-year")]
        public async Task<IActionResult> getAcademicYear(int academic_year_id)
        {
            if (academic_year_id == null) { return BadRequest(new { Message = "Data is provided is null" }); }

            var academicYear = await _context.Academic_Years.FirstOrDefaultAsync(a => a.academic_year_id == academic_year_id);

            if (academicYear != null) { return BadRequest(new { Message = "Academic Year is already exist" }); }

            return Ok(academicYear);

        }

        [HttpPost("add-academic-year")]
        public async Task<IActionResult> addAcademicYears(Academic_Years academic_Years)
        {
            if(academic_Years == null) { return BadRequest(new {Message = "Data is provided is null"}); }

            bool anyAcademicYearIsNull = typeof(Academic_Years).GetProperties().Any(prop =>
            {
                return prop.GetValue(academic_Years) == null;
            });

            if (anyAcademicYearIsNull) { return BadRequest(new { Message = "Data is provided have a property is null" }); }

            if(academic_Years.academic_Year_startClosureDate >= academic_Years.academic_Year_endClosureDate )
            {
                return BadRequest(new { Message = "Start Closure Date cannot equal or grater than End Closure Date" });
            }

            await _context.AddAsync(academic_Years);
            await _context.SaveChangesAsync();

            return Ok(new {Message = "Add new Academic Year successfully"});
        }

        [HttpPost("update-academic-year")]
        public async Task<IActionResult> updateAcademicYear(Academic_Years academicYears)
        {
            if(academicYears == null) { return BadRequest(new { Message = "Data is provided is null" }); }

            bool academicYearIsNull = typeof(Academic_Years).GetProperties().Any(prop =>
            {
                return prop.GetValue(academicYears) == null;
            });

            if (academicYearIsNull)
            {
                return BadRequest(new { Message = "Data is provided have a property is null" });
            }

            var checkAcademicYear = await _context.Academic_Years.FirstOrDefaultAsync(a => a.academic_year_id == academicYears.academic_year_id);
            
            if (checkAcademicYear == null)
            {
                return BadRequest(new { Message = "Academic Year is not found" });
            }

            if(academicYears.academic_Year_startClosureDate >= academicYears.academic_Year_endClosureDate)
            {
                return BadRequest(new { Message = "Start Closure Date cannot equal or grater than End Closure Date" });
            }

            _context.Entry(academicYears).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Update Academic Year successfully" });
        }

    }
}
