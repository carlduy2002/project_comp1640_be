using Microsoft.AspNetCore.Http;
﻿using Microsoft.AspNetCore.Authorization;
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
            var lstFaculty = _context.Faculties
                .Where(a => !a.faculty_name.Contains("Account"))
                .ToList();

            if (lstFaculty.Count == 0)
                return NotFound();

            return Ok(lstFaculty);
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
            if (faculties == null)
                return BadRequest(new { Message = "Data to add is null" });

            if (await checkExistFaculty(faculties.faculty_name))
                return BadRequest(new { Message = "Faculty already exist" });

            await _context.Faculties.AddAsync(faculties);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Add Faculty Succeed" });
        }
        private Task<bool> checkExistFaculty(string faculty_name)
        {
            return _context.Faculties.AnyAsync(f => f.faculty_name == faculty_name);
        }

        [HttpGet("get-faculty")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> getFaculty(int faculty_id)
        {
            if (faculty_id == null) { return BadRequest(new { Message = "Data is provided is null" }); }

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

            return Ok(new { Message = "Update faculty successfull" });
        }

        [HttpDelete]
        public async Task<IActionResult> deleteFAculty(int faculty_id)
        {
            var faculty = _context.Faculties.Where(f => f.faculty_id.Equals(faculty_id)).FirstOrDefault();

            if (faculty == null)
                return NotFound(new { Message = "Faculty is not found" });

            _context.Faculties.Remove(faculty);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Delete Fculty Succeed" });
        }
    }
}
