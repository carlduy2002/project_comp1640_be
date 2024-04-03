using Aspose.Words.Bibliography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neo4jClient.DataAnnotations.Cypher.Functions;
using project_comp1640_be.Data;
using project_comp1640_be.Model;
using SkiaSharp;

namespace project_comp1640_be.Controllers
{
    [Route("statisticals")]
    [ApiController]
    public class StatisticalsController : ControllerBase
    {
        readonly ApplicationDbContext _context;

        public StatisticalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        //[HttpGet]
        //public async Task<IActionResult> numberOfContributions(string academic)
        //{
        //    if (academic == null)
        //        return BadRequest();

        //    var academicYearId = _context.Academic_Years
        //        .Where(a => a.academic_year_title.Contains(academic))
        //        .Select(a => a.academic_year_id)
        //        .FirstOrDefault();

        //    if (academicYearId == null)
        //        return BadRequest();

        //    var lstContributions = _context.Contributions
        //            .Where(c => c.contribution_academic_years_id == academicYearId)
        //            .Count();

        //    if (lstContributions == null)
        //        return BadRequest();

        //    return Ok(lstContributions);
        //}

        //[HttpGet("Contributor")]
        //public async Task<IActionResult> numberOfContributor(string academic)
        //{
        //    if (academic == null)
        //        return BadRequest();

        //    var academicYearId = _context.Academic_Years
        //        .Where(a => a.academic_year_title.Contains(academic))
        //        .Select(a => a.academic_year_id)
        //        .FirstOrDefault();

        //    var contributorIds = _context.Contributions
        //        .GroupBy(c => c.contribution_user_id)
        //        .Select(group => group.Key)
        //        .ToList();


        //    var lstContributor = _context.Contributions
        //        .Where(c => c.contribution_academic_years_id == academicYearId)
        //        .GroupBy(c => c.contribution_user_id)
        //        .Select(group => group.Key)
        //        .Count();

        //    if(lstContributor == null)
        //        return BadRequest();

        //    return Ok(lstContributor);
        //}

        [HttpGet("guest-statistic")]
        public async Task<IActionResult> guestStatistic(int academic_year_id)
        {
            if (academic_year_id == null) { return BadRequest(new { Message = "Academic Year ID is null" }); }

            var faculties = await _context.Faculties.ToListAsync();
            List<Object> academic = new List<Object>();

            foreach (var facultie in faculties)
            {
                var dataStatisticByAcademic = _context.Academic_Years
                    .Where(y => y.academic_year_id == academic_year_id)
                    .Select(y => new  
                    {
                        ContributionImages = _context.Contributions
                            .Where(c => c.contribution_academic_years_id == y.academic_year_id
                                && c.users.user_faculty_id == facultie.faculty_id)
                            .Select(c => c.contribution_image)
                            .FirstOrDefault(),
                        StartDate = y.academic_year_ClosureDate,
                        EndDate = y.academic_year_FinalClosureDate, 
                        Contributions = _context.Contributions
                            .Count(c => c.contribution_academic_years_id == y.academic_year_id
                                && c.users.user_faculty_id == facultie.faculty_id), 
                        UsersCount = _context.Contributions
                            .Where(c => c.users.user_faculty_id == facultie.faculty_id 
                                && c.contribution_academic_years_id == y.academic_year_id)
                            .GroupBy(c => new { c.contribution_user_id, c.contribution_academic_years_id })
                            .Count(),
                        FacultyName = facultie.faculty_name 
                    }).FirstOrDefault();

                academic.Add(dataStatisticByAcademic);
            }

            return Ok(academic);
        }

        [HttpGet("coordinator-statistic")]
        public async Task<IActionResult> coordinatorStatistic(int academic_year_id)
        {
            if (academic_year_id == null) { return BadRequest(new { Message = "Academic Year ID is null" }); }

            var dataStatisticByAcademic = _context.Academic_Years
                .Where(y => y.academic_year_id == academic_year_id)
                .Select(y => new
                {
                    academic_year_id = y.academic_year_id,
                    title = y.academic_year_title,
                    StartDate = y.academic_year_ClosureDate,
                    EndDate = y.academic_year_FinalClosureDate,
                    usersCount = y.contributions
                            .Where(c => c.contribution_academic_years_id == y.academic_year_id)
                            .GroupBy(c => c.users.user_id)
                            .Count(),
                    contribution = y.contributions.Where(c => c.contribution_academic_years_id == y.academic_year_id).Count(),
                }).ToList();

            return Ok(dataStatisticByAcademic);
        }

        [HttpGet("admin-statistic")]
        public async Task<IActionResult> adminStatistic(int academic_year_id)
        {
            if (academic_year_id == null) { return BadRequest(new { Message = "Academic Year ID is null" }); }

            var faculties = await _context.Faculties.ToListAsync();
            List<Object> academic = new List<Object>();

            foreach (var facultie in faculties)
            {
                var dataStatisticByAcademic = _context.Academic_Years
                    .Where(y => y.academic_year_id == academic_year_id)
                    .Select(y => new
                    {
                        ContributionImages = _context.Contributions
                            .Where(c => c.contribution_academic_years_id == y.academic_year_id
                                && c.users.user_faculty_id == facultie.faculty_id)
                            .Select(c => c.contribution_image)
                            .FirstOrDefault(),
                        StartDate = y.academic_year_ClosureDate,
                        EndDate = y.academic_year_FinalClosureDate,
                        Contributions = _context.Contributions
                            .Count(c => c.contribution_academic_years_id == y.academic_year_id
                                && c.users.user_faculty_id == facultie.faculty_id),
                        UsersCount = _context.Contributions
                            .Where(c => c.users.user_faculty_id == facultie.faculty_id
                                && c.contribution_academic_years_id == y.academic_year_id)
                            .GroupBy(c => new { c.contribution_user_id, c.contribution_academic_years_id })
                            .Count(),
                        FacultyName = facultie.faculty_name,
                        entireFaculties = Math.Round((double)_context.Contributions
                            .Where(c => c.users.user_faculty_id == facultie.faculty_id
                                && c.contribution_academic_years_id == y.academic_year_id)
                            .GroupBy(c => new { c.contribution_user_id, c.contribution_academic_years_id })
                            .Count()/_context.Contributions
                            .Where(c => c.contribution_academic_years_id == y.academic_year_id)
                            .GroupBy(c => new { c.contribution_user_id, c.contribution_academic_years_id })
                            .Count()*100, 2),
                        entireContribution = Math.Round((double)_context.Contributions
                            .Count(c => c.contribution_academic_years_id == y.academic_year_id
                                && c.users.user_faculty_id == facultie.faculty_id)/ _context.Contributions
                            .Count(c => c.contribution_academic_years_id == y.academic_year_id)*100, 2)
                    }).FirstOrDefault();

                academic.Add(dataStatisticByAcademic);
            }

            return Ok(academic);
        }

        [HttpGet("before-statistic")]
        public async Task<IActionResult> beforeStatistic(int academic_year_id)
        {
            if (academic_year_id == null) { return BadRequest(new { Message = "Academic Year ID is null" }); }

            var lstContribution = _context.Marketing_Comments
                .Where(m => m.comment_contribution_id == m.contributions.contribution_id
                    && m.comment_date > m.contributions.contribution_submition_date.AddDays(14)
                    && m.contributions.contribution_academic_years_id == academic_year_id)
                .Select(m => m.comment_contribution_id).ToList();

            if(lstContribution == null)
            {
                return Ok();
            }

            List<Object> dataStatistic = new List<Object>();

            foreach(var item in lstContribution) 
            {
                var contribution = _context.Contributions
                    .Where(c => c.contribution_id == item)
                    .Select(c => new
                    {
                        contributionImages = c.contribution_image,
                        contributionID = c.contribution_id,
                        contributionTitle = c.contribution_title,
                        contributionSubmit = c.contribution_submition_date,
                        StartDate = c.academic_years.academic_year_ClosureDate,
                        EndDate = c.academic_years.academic_year_FinalClosureDate,
                        contributor = _context.Users
                            .Where(u => u.user_id == c.contribution_user_id)
                            .Select(u => u.user_username).FirstOrDefault(),
                        faculty = _context.Users
                            .Where(u => u.user_id == c.contribution_user_id)
                            .Select(u => u.faculties.faculty_name).FirstOrDefault(),
                        coordinator = _context.Users
                            .Where(u => u.role.role_name == "Coordinator" && u.faculties.faculty_id == c.users.user_faculty_id)
                            .Select(u => u.user_username).FirstOrDefault()
                    }).FirstOrDefault();
                dataStatistic.Add(contribution);
            }

            return Ok(dataStatistic);
        }

        [HttpGet("after-statistic")]
        public async Task<IActionResult> afterStatistic(int academic_year_id)
        {
            if (academic_year_id == null) { return BadRequest(new { Message = "Academic Year ID is null" }); }

            var lstContribution = _context.Contributions
                .Where(contribution => !_context.Marketing_Comments
                    .Any(comment => comment.comment_contribution_id == contribution.contribution_id)
                    && contribution.contribution_academic_years_id == academic_year_id)
                .Select(contribution => contribution.contribution_id)
                .ToList();

            if (lstContribution == null)
            {
                return Ok();
            }

            List<Object> dataStatistic = new List<Object>();

            foreach (var item in lstContribution)
            {
                var contribution = _context.Contributions
                    .Where(c => c.contribution_id == item)
                    .Select(c => new
                    {
                        contributionImages = c.contribution_image,
                        contributionID = c.contribution_id,
                        contributionTitle = c.contribution_title,
                        contributionSubmit = c.contribution_submition_date,
                        StartDate = c.academic_years.academic_year_ClosureDate,
                        EndDate = c.academic_years.academic_year_FinalClosureDate,
                        contributor = _context.Users
                            .Where(u => u.user_id == c.contribution_user_id)
                            .Select(u => u.user_username).FirstOrDefault(),
                        faculty = _context.Users
                            .Where(u => u.user_id == c.contribution_user_id)
                            .Select(u => u.faculties.faculty_name).FirstOrDefault(),
                        coordinator = _context.Users
                            .Where(u => u.role.role_name == "Coordinator" && u.faculties.faculty_id == c.users.user_faculty_id)
                            .Select(u => u.user_username).FirstOrDefault()
                    }).FirstOrDefault();
                dataStatistic.Add(contribution);
            }

            return Ok(dataStatistic);
        }


        [HttpGet("statistical_approve_reject")]
        public async Task<IActionResult> statistical_approve_reject(int academic_id, int faculty_id)
        {
            if (academic_id == null)
                return BadRequest();

            if(faculty_id == null)
                return BadRequest();

            var roleID = _context.Roles.Where(r => r.role_name.Equals("Coordinator")).Select(r => r.role_id).FirstOrDefault();

            var TotalContribution = (
                    from c in _context.Contributions
                    join u in _context.Users on c.contribution_user_id equals u.user_id
                    join f in _context.Faculties on u.user_faculty_id equals f.faculty_id
                    where c.contribution_academic_years_id == academic_id && f.faculty_id == faculty_id
                    select c.contribution_id
                ).Count();

            var TotalContributor = _context.Users
                        .Where(u => u.user_faculty_id == faculty_id)
                        .Select(u => u.user_id)
                        .Count();


            var statisticalDataContributions = _context.Contributions
                .Where(c => c.contribution_academic_years_id == academic_id)
                .Select(c => new
                {
                    faculty_name = _context.Faculties
                        .Where(f => f.faculty_id == faculty_id)
                        .Select(f => f.faculty_name)
                        .FirstOrDefault(),

                    CoordinatorName = _context.Users
                        .Where(u => u.user_faculty_id == faculty_id && u.user_role_id == roleID)
                        .Select(u => u.user_username)
                        .FirstOrDefault(),

                    NumberContributor = TotalContributor,

                    RateSubmit = (((double)TotalContribution / TotalContributor) * 100).ToString("0.00"),

                    NumberContribution = TotalContribution,

                    NumberContributionApproved = _context.Contributions
                        .Where(c => c.contribution_academic_years_id == academic_id
                            && c.users.user_faculty_id == faculty_id && c.IsSelected.Equals(IsSelected.Selected))
                        .Select(c => c.contribution_id)
                        .Count(),

                    NumberContributionRejected = _context.Contributions
                        .Where(c => c.contribution_academic_years_id == academic_id
                            && c.users.user_faculty_id == faculty_id && c.IsSelected.Equals(IsSelected.Unselected))
                        .Select(c => c.contribution_id)
                        .Count(),

                    RateContributionApproved = ((_context.Contributions
                        .Where(c => c.contribution_academic_years_id == academic_id
                            && c.users.user_faculty_id == faculty_id && c.IsSelected.Equals(IsSelected.Selected))
                        .Count() / (double)TotalContribution) * 100).ToString("0.00"),

                    RateContributionRejected = ((_context.Contributions
                        .Where(c => c.contribution_academic_years_id == academic_id
                            && c.users.user_faculty_id == faculty_id && c.IsSelected.Equals(IsSelected.Unselected))
                        .Count() / (double)TotalContribution) * 100).ToString("0.00"),

                }).FirstOrDefault();

            List<object> st = new List<object>();

            st.Add(statisticalDataContributions);


            return Ok(st);
        }


        [HttpGet("statistical_approve_reject_chart")]
        public async Task<IActionResult> statistical_approve_reject_chart(int academic_id)
        {

            var faculty = _context.Faculties
                .Where(f => !f.faculty_name.Contains("Admin_Account") && !f.faculty_name.Contains("Manager_Account"))
                .Select(f => new
                {
                    f.faculty_id,
                    f.faculty_name
                })
                .ToList();

            var roleID = _context.Roles.Where(r => r.role_name.Equals("Coordinator")).Select(r => r.role_id).FirstOrDefault();

            var TotalFacultyInYear = (
                    from f in _context.Faculties
                    join u in _context.Users on f.faculty_id equals u.user_faculty_id
                    join c in _context.Contributions on u.user_id equals c.contribution_user_id
                    join a in _context.Academic_Years on c.contribution_academic_years_id equals a.academic_year_id
                    where a.academic_year_id == academic_id && !f.faculty_name.Contains("Admin_Account") && !f.faculty_name.Contains("Manager_Account")
                    select new { f.faculty_name, f.faculty_id }
                ).ToList();

            List<object> st = new List<object>();

            foreach(var i in faculty)
            {
                var TotalContributor = _context.Users
                        .Where(u => u.user_faculty_id == i.faculty_id)
                        .Select(u => u.user_id)
                        .Count();

                var TotalContribution = (
                        from c in _context.Contributions
                        join u in _context.Users on c.contribution_user_id equals u.user_id
                        join f in _context.Faculties on u.user_faculty_id equals f.faculty_id
                        where c.contribution_academic_years_id == academic_id && f.faculty_id == i.faculty_id
                        select c.contribution_id).Count();

                var statisticalDataContributions = _context.Contributions
                .Where(c => c.contribution_academic_years_id == academic_id)
                .Select(c => new
                {
                    faculty_name = i.faculty_name,

                    NumberContributor = TotalContributor,

                    NumberContribution = TotalContribution,

                    NumberContributionApproved = _context.Contributions
                        .Where(c => c.contribution_academic_years_id == academic_id
                            && c.users.user_faculty_id == i.faculty_id && c.IsSelected.Equals(IsSelected.Selected))
                        .Select(c => c.contribution_id)
                        .Count(),

                    NumberContributionRejected = _context.Contributions
                        .Where(c => c.contribution_academic_years_id == academic_id
                            && c.users.user_faculty_id == i.faculty_id && c.IsSelected.Equals(IsSelected.Unselected))
                        .Select(c => c.contribution_id)
                        .Count(),

                }).FirstOrDefault();

                st.Add(statisticalDataContributions);
            }
            
            return Ok(st);
        }


    }
}
