using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neo4jClient.DataAnnotations.Cypher.Functions;
using project_comp1640_be.Data;
using project_comp1640_be.Model;

namespace project_comp1640_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticalsController : ControllerBase
    {
        readonly ApplicationDbContext _context;

        public StatisticalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> numberOfContributions(string academic)
        {
            if (academic == null)
                return BadRequest();

            var academicYearId = _context.Academic_Years
                .Where(a => a.academic_year_title.Contains(academic))
                .Select(a => a.academic_year_id)
                .FirstOrDefault();

            if (academicYearId == null)
                return BadRequest();

            var lstContributions = _context.Contributions
                    .Where(c => c.contribution_academic_years_id == academicYearId)
                    .Count();

            if (lstContributions == null)
                return BadRequest();

            return Ok(lstContributions);
        }

        [HttpGet("Contributor")]
        public async Task<IActionResult> numberOfContributor(string academic)
        {
            if (academic == null)
                return BadRequest();

            var academicYearId = _context.Academic_Years
                .Where(a => a.academic_year_title.Contains(academic))
                .Select(a => a.academic_year_id)
                .FirstOrDefault();

            var contributorIds = _context.Contributions
                .GroupBy(c => c.contribution_user_id)
                .Select(group => group.Key)
                .ToList();


            var lstContributor = _context.Contributions
                .Where(c => c.contribution_academic_years_id == academicYearId)
                .GroupBy(c => c.contribution_user_id)
                .Select(group => group.Key)
                .Count();

            if(lstContributor == null)
                return BadRequest();

            return Ok(lstContributor);
        }

    }
}
