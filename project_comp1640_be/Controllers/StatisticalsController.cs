//using Aspose.Words.Bibliography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neo4jClient.DataAnnotations.Cypher.Functions;
using project_comp1640_be.Data;
using project_comp1640_be.Model;
//using SkiaSharp;
using System.Linq;

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

        [HttpGet("guest-statistic")]
        public async Task<IActionResult> guestStatistic(int academic_year_id, string username)
        {
            if (academic_year_id == null) { return BadRequest(new { Message = "Academic Year ID is null" }); }

            if (username == null) { return BadRequest(new { Message = "Data provided is null" }); }

            var getFaculty = _context.Users.Where(u => u.user_username == username).Select(u => u.user_faculty_id).FirstOrDefault();

            if (academic_year_id == 0)
            {
                var dataStatistic = _context.Academic_Years
                    .Select(y => new
                    {
                        ContributionImages = _context.Contributions
                                                .Where(c => c.users.user_faculty_id == getFaculty
                                                        && c.contribution_academic_years_id == y.academic_year_id)
                                                .Select(c => c.contribution_image)
                                                .FirstOrDefault(),
                        academic_year_title = y.academic_year_title,
                        StartDate = y.academic_year_ClosureDate,
                        EndDate = y.academic_year_FinalClosureDate,
                        Contributions = _context.Contributions
                                            .Count(c => c.users.user_faculty_id == getFaculty
                                                        && c.contribution_academic_years_id == y.academic_year_id),
                        UsersCount = _context.Contributions
                                        .Where(c => c.users.user_faculty_id == getFaculty
                                                    && c.contribution_academic_years_id == y.academic_year_id)
                                        .GroupBy(c => c.contribution_user_id)
                                        .Count(),
                        FacultyName = _context.Faculties.Where(f => f.faculty_id == getFaculty).Select(y => y.faculty_name).FirstOrDefault()
                    }).ToList();
                return Ok(dataStatistic);
            }

            var data = _context.Academic_Years
                                    .Where(y => y.academic_year_id == academic_year_id)
                                    .Select(y => new
                                    {
                                        ContributionImages = _context.Contributions
                                                                .Where(c => c.users.user_faculty_id == getFaculty
                                                                            && c.contribution_academic_years_id == y.academic_year_id)
                                                                .Select(c => c.contribution_image)
                                                                .FirstOrDefault(),
                                        StartDate = y.academic_year_ClosureDate,
                                        EndDate = y.academic_year_FinalClosureDate,
                                        Contributions = _context.Contributions
                                                            .Count(c => c.users.user_faculty_id == getFaculty
                                                                        && c.contribution_academic_years_id == y.academic_year_id),
                                        UsersCount = _context.Contributions
                                                        .Where(c => c.users.user_faculty_id == getFaculty
                                                                        && c.contribution_academic_years_id == y.academic_year_id)
                                                        .GroupBy(c => c.contribution_user_id)
                                                        .Count(),
                                        FacultyName = _context.Faculties.Where(f => f.faculty_id == getFaculty).Select(y => y.faculty_name).FirstOrDefault()
                                    }).ToList();

            //var dataStatisticByAcademic = _context.Academic_Years
            //    .Where(y => y.academic_year_id == academic_year_id)
            //    .Select(y => new  
            //    {
            //        ContributionImages = _context.Contributions
            //            .Where(c => c.contribution_academic_years_id == y.academic_year_id
            //                && c.users.user_faculty_id == facultie.faculty_id)
            //            .Select(c => c.contribution_image)
            //            .FirstOrDefault(),
            //        StartDate = y.academic_year_ClosureDate,
            //        EndDate = y.academic_year_FinalClosureDate, 
            //        Contributions = _context.Contributions
            //            .Count(c => c.contribution_academic_years_id == y.academic_year_id
            //                && c.users.user_faculty_id == facultie.faculty_id), 
            //        UsersCount = _context.Contributions
            //            .Where(c => c.users.user_faculty_id == facultie.faculty_id 
            //                && c.contribution_academic_years_id == y.academic_year_id)
            //            .GroupBy(c => new { c.contribution_user_id, c.contribution_academic_years_id })
            //            .Count(),
            //        FacultyName = facultie.faculty_name 
            //    }).FirstOrDefault();

            return Ok(data);
        }

        [HttpGet("coordinator-statistic")]
        public async Task<IActionResult> coordinatorStatistic(int academic_year_id, string username)
        {
            if (academic_year_id == null) { return BadRequest(new { Message = "Academic Year ID is null" }); }

            var getFacultyID = _context.Users.Where(u => u.user_username == username)
                                                .Select(u => u.faculties.faculty_id).FirstOrDefault();

            if (academic_year_id == 0)
            {
                var dataStatistic = _context.Academic_Years
                .Select(y => new
                {
                    academic_year_id = y.academic_year_id,
                    title = y.academic_year_title,
                    StartDate = y.academic_year_ClosureDate,
                    EndDate = y.academic_year_FinalClosureDate,
                    usersCount = y.contributions
                            .Where(c => c.contribution_academic_years_id == y.academic_year_id
                                            && c.users.user_faculty_id == getFacultyID)
                            .GroupBy(c => c.users.user_id)
                            .Count(),
                    contribution = y.contributions.Where(c => c.contribution_academic_years_id == y.academic_year_id
                                                                && c.users.user_faculty_id == getFacultyID).Count(),
                }).ToList();

                return Ok(dataStatistic);
            }

            var dataStatisticByAcademic = _context.Academic_Years
                .Where(y => y.academic_year_id == academic_year_id)
                .Select(y => new
                {
                    academic_year_id = y.academic_year_id,
                    title = y.academic_year_title,
                    StartDate = y.academic_year_ClosureDate,
                    EndDate = y.academic_year_FinalClosureDate,
                    usersCount = y.contributions
                            .Where(c => c.contribution_academic_years_id == y.academic_year_id
                                            && c.users.user_faculty_id == getFacultyID)
                            .GroupBy(c => c.users.user_id)
                            .Count(),
                    contribution = y.contributions.Where(c => c.contribution_academic_years_id == y.academic_year_id
                                                                && c.users.user_faculty_id == getFacultyID).Count(),
                }).ToList();

            return Ok(dataStatisticByAcademic);
        }

        [HttpGet("admin-statistic")]
        public async Task<IActionResult> adminStatistic(int academic_year_id)
        {
            if (academic_year_id == null) { return BadRequest(new { Message = "Academic Year ID is null" }); }

            var faculties = _context.Faculties
                        .Where(fa => fa.faculty_name != "Admin_Account" && fa.faculty_name != "Manager_Account").ToList();

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
                            .Count() / _context.Contributions
                            .Where(c => c.contribution_academic_years_id == y.academic_year_id)
                            .GroupBy(c => new { c.contribution_user_id, c.contribution_academic_years_id })
                            .Count() * 100),
                        entireContribution = Math.Round((double)_context.Contributions
                            .Count(c => c.contribution_academic_years_id == y.academic_year_id
                                && c.users.user_faculty_id == facultie.faculty_id) / _context.Contributions
                            .Count(c => c.contribution_academic_years_id == y.academic_year_id) * 100)
                    }).FirstOrDefault();

                academic.Add(dataStatisticByAcademic);
            }

            return Ok(academic);
        }

        [HttpGet("before-statistic")]
        public async Task<IActionResult> beforeStatistic(int academic_year_id)
        {
            if (academic_year_id == null)
            {
                return BadRequest(new { Message = "Academic Year ID is null" });
            }

            var lstContribution = _context.Contributions
                .Where(c => c.contribution_submition_date >= DateTime.Now.AddDays(-14)
                         && c.contribution_academic_years_id == academic_year_id
                         && !_context.Marketing_Comments
                                .Any(mc => mc.comment_contribution_id == c.contribution_id)
                         && _context.Users
                                .Any(u => u.role.role_name == "Coordinator"
                                         && u.user_faculty_id == c.users.user_faculty_id))
                .ToList();

            if (lstContribution.Count == 0)
            {
                return Ok();
            }

            List<object> dataStatistic = new List<object>();

            foreach (var item in lstContribution)
            {
                var contribution = _context.Contributions
                    .Where(c => c.contribution_id == item.contribution_id)
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
                            .Select(u => u.user_username)
                            .FirstOrDefault(),
                        faculty = _context.Users
                            .Where(u => u.user_id == c.contribution_user_id)
                            .Select(u => u.faculties.faculty_name)
                            .FirstOrDefault(),
                        coordinator = _context.Users
                            .Where(u => u.role.role_name == "Coordinator" && u.faculties.faculty_id == c.users.user_faculty_id)
                            .Select(u => u.user_username)
                            .FirstOrDefault()
                    }).FirstOrDefault();

                dataStatistic.Add(contribution);
            }

            return Ok(dataStatistic);
        }

        [HttpGet("after-statistic")]
        public async Task<IActionResult> afterStatistic(int academic_year_id)
        {
            if (academic_year_id == null)
            {
                return BadRequest(new { Message = "Academic Year ID is null" });
            }

            var lstContribution = _context.Contributions
                .Where(c => c.contribution_submition_date.AddDays(14) < DateTime.Now
                         && c.contribution_academic_years_id == academic_year_id
                         && !_context.Marketing_Comments
                                .Any(mc => mc.comment_contribution_id == c.contribution_id)
                         && _context.Users
                                .Any(u => u.role.role_name == "Coordinator"
                                         && u.user_faculty_id == c.users.user_faculty_id))
                .ToList();

            if (lstContribution.Count == 0)
            {
                return Ok();
            }

            List<object> dataStatistic = new List<object>();

            foreach (var item in lstContribution)
            {
                var contribution = _context.Contributions
                    .Where(c => c.contribution_id == item.contribution_id)
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
                            .Select(u => u.user_username)
                            .FirstOrDefault(),
                        faculty = _context.Users
                            .Where(u => u.user_id == c.contribution_user_id)
                            .Select(u => u.faculties.faculty_name)
                            .FirstOrDefault(),
                        coordinator = _context.Users
                            .Where(u => u.role.role_name == "Coordinator" && u.faculties.faculty_id == c.users.user_faculty_id)
                            .Select(u => u.user_username)
                            .FirstOrDefault()
                    }).FirstOrDefault();

                dataStatistic.Add(contribution);
            }

            return Ok(dataStatistic);
        }

        //start
        [HttpGet("statistical_approve_reject")]
        public async Task<IActionResult> statistical_approve_reject(int academic_id)
        {
            if (academic_id == null)
                return BadRequest();

            List<object> st = new List<object>();

            var getAllFaculties = _context.Faculties
                .Where(f => !f.faculty_name.Equals("None"))
                .ToList();

            if (academic_id != 0)
            {
                var roleID = _context.Roles.Where(r => r.role_name.Equals("Coordinator")).Select(r => r.role_id).FirstOrDefault();

                foreach (var f in getAllFaculties)
                {
                    var TotalContribution = (
                        from c in _context.Contributions
                        join u in _context.Users on c.contribution_user_id equals u.user_id
                        join fa in _context.Faculties on u.user_faculty_id equals fa.faculty_id
                        where c.contribution_academic_years_id == academic_id && fa.faculty_id == f.faculty_id
                        select c.contribution_id
                    ).Count();

                    var TotalContributor = _context.Users
                            .Include(u => u.role)
                            .Where(u => u.role.role_name.Equals("Student") && u.user_faculty_id == f.faculty_id)
                            .Select(u => u.user_id)
                            .Count();

                    if(TotalContribution != 0) 
                    {
                        var statisticalDataContributions = _context.Contributions
                        .Where(c => c.contribution_academic_years_id == academic_id)
                        .Select(c => new
                        {
                            faculty_name = f.faculty_name,

                            CoordinatorName = _context.Users
                                .Where(u => u.user_faculty_id == f.faculty_id && u.user_role_id == roleID)
                                .Select(u => u.user_username)
                                .FirstOrDefault(),

                            NumberContributor = TotalContributor,

                            RateSubmit = (((double)TotalContribution / TotalContributor) * 100).ToString("0.00"),

                            NumberContribution = TotalContribution,

                            NumberContributionApproved = _context.Contributions
                                .Where(c => c.contribution_academic_years_id == academic_id
                                    && c.users.user_faculty_id == f.faculty_id && c.IsSelected.Equals(IsSelected.Selected.ToString()))
                                .Select(c => c.contribution_id)
                                .Count(),

                            NumberContributionRejected = _context.Contributions
                                .Where(c => c.contribution_academic_years_id == academic_id
                                    && c.users.user_faculty_id == f.faculty_id && c.IsSelected.Equals(IsSelected.Unselected.ToString()))
                                .Select(c => c.contribution_id)
                                .Count(),

                            RateContributionApproved = ((_context.Contributions
                                .Where(c => c.contribution_academic_years_id == academic_id
                                    && c.users.user_faculty_id == f.faculty_id && c.IsSelected.Equals(IsSelected.Selected.ToString()))
                                .Count() / (double)TotalContribution) * 100).ToString("0.00"),

                            RateContributionRejected = ((_context.Contributions
                                .Where(c => c.contribution_academic_years_id == academic_id
                                    && c.users.user_faculty_id == f.faculty_id && c.IsSelected.Equals(IsSelected.Unselected.ToString()))
                                .Count() / (double)TotalContribution) * 100).ToString("0.00"),

                            academic = _context.Academic_Years
                                .Where(a => a.academic_year_id == academic_id)
                                .Select(a => a.academic_year_title)
                                .FirstOrDefault()

                        }).FirstOrDefault();

                        st.Add(statisticalDataContributions);
                    }
                    else
                    {
                        var statisticalDataContributions = _context.Contributions
                        .Where(c => c.contribution_academic_years_id == academic_id)
                        .Select(c => new
                        {
                            faculty_name = f.faculty_name,

                            CoordinatorName = _context.Users
                                .Where(u => u.user_faculty_id == f.faculty_id && u.user_role_id == roleID)
                                .Select(u => u.user_username)
                                .FirstOrDefault(),

                            NumberContributor = TotalContributor,

                            RateSubmit = 0,

                            NumberContribution = TotalContribution,

                            NumberContributionApproved = _context.Contributions
                                .Where(c => c.contribution_academic_years_id == academic_id
                                    && c.users.user_faculty_id == f.faculty_id && c.IsSelected.Equals(IsSelected.Selected.ToString()))
                                .Select(c => c.contribution_id)
                                .Count(),

                            NumberContributionRejected = _context.Contributions
                                .Where(c => c.contribution_academic_years_id == academic_id
                                    && c.users.user_faculty_id == f.faculty_id && c.IsSelected.Equals(IsSelected.Unselected.ToString()))
                                .Select(c => c.contribution_id)
                                .Count(),

                            RateContributionApproved = 0,

                            RateContributionRejected = 0,
                            academic = _context.Academic_Years
                                .Where(a => a.academic_year_id == academic_id)
                                .Select(a => a.academic_year_title)
                                .FirstOrDefault()

                        }).FirstOrDefault();

                        st.Add(statisticalDataContributions);
                    }

                    
                }
            }
            else
            {
                var getMaxAcademic = _context.Academic_Years.Max(a => a.academic_year_id);

                var roleID = _context.Roles.Where(r => r.role_name.Equals("Coordinator")).Select(r => r.role_id).FirstOrDefault();

                foreach (var f in getAllFaculties)
                {
                    var TotalContribution = (
                       from c in _context.Contributions
                       join u in _context.Users on c.contribution_user_id equals u.user_id
                       join fa in _context.Faculties on u.user_faculty_id equals fa.faculty_id
                       where c.contribution_academic_years_id == getMaxAcademic && fa.faculty_id == f.faculty_id
                       select c.contribution_id
                        ).Count();

                    var TotalContributor = _context.Users
                            .Include(u => u.role)
                            .Where(u => u.role.role_name.Equals("Student") && u.user_faculty_id == f.faculty_id)
                            .Select(u => u.user_id)
                            .Count();

                    if(TotalContribution != 0)
                    {
                        var statisticalDataContributions = _context.Contributions
                        .Where(c => c.contribution_academic_years_id == getMaxAcademic)
                        .Select(c => new
                        {
                            faculty_name = _context.Faculties
                                .Where(faculties => faculties.faculty_id == f.faculty_id)
                                .Select(faculties => faculties.faculty_name)
                                .FirstOrDefault(),

                            CoordinatorName = _context.Users
                                .Where(u => u.user_faculty_id == f.faculty_id && u.user_role_id == roleID)
                                .Select(u => u.user_username)
                                .FirstOrDefault(),

                            NumberContributor = TotalContributor,

                            RateSubmit = (((double)TotalContribution / TotalContributor) * 100).ToString("0.00"),

                            NumberContribution = TotalContribution,

                            NumberContributionApproved = _context.Contributions
                                .Where(c => c.contribution_academic_years_id == getMaxAcademic
                                    && c.users.user_faculty_id == f.faculty_id && c.IsSelected.Equals(IsSelected.Selected.ToString()))
                                .Select(c => c.contribution_id)
                                .Count(),

                            NumberContributionRejected = _context.Contributions
                                .Where(c => c.contribution_academic_years_id == getMaxAcademic
                                    && c.users.user_faculty_id == f.faculty_id && c.IsSelected.Equals(IsSelected.Unselected.ToString()))
                                .Select(c => c.contribution_id)
                                .Count(),

                            RateContributionApproved = ((_context.Contributions
                                .Where(c => c.contribution_academic_years_id == getMaxAcademic
                                    && c.users.user_faculty_id == f.faculty_id && c.IsSelected.Equals(IsSelected.Selected.ToString()))
                                .Count() / (double)TotalContribution) * 100).ToString("0.00"),

                            RateContributionRejected = ((_context.Contributions
                                .Where(c => c.contribution_academic_years_id == getMaxAcademic
                                    && c.users.user_faculty_id == f.faculty_id && c.IsSelected.Equals(IsSelected.Unselected.ToString()))
                                .Count() / (double)TotalContribution) * 100).ToString("0.00"),

                            academic = _context.Academic_Years
                                .Where(a => a.academic_year_id == getMaxAcademic)
                                .Select(a => a.academic_year_title)
                                .FirstOrDefault()

                        }).FirstOrDefault();

                        st.Add(statisticalDataContributions);
                    }
                    else
                    {
                        var statisticalDataContributions = _context.Contributions
                        .Where(c => c.contribution_academic_years_id == getMaxAcademic)
                        .Select(c => new
                        {
                            faculty_name = _context.Faculties
                                .Where(faculties => faculties.faculty_id == f.faculty_id)
                                .Select(faculties => faculties.faculty_name)
                                .FirstOrDefault(),

                            CoordinatorName = _context.Users
                                .Where(u => u.user_faculty_id == f.faculty_id && u.user_role_id == roleID)
                                .Select(u => u.user_username)
                                .FirstOrDefault(),

                            NumberContributor = TotalContributor,

                            RateSubmit = 0,

                            NumberContribution = TotalContribution,

                            NumberContributionApproved = _context.Contributions
                                .Where(c => c.contribution_academic_years_id == getMaxAcademic
                                    && c.users.user_faculty_id == f.faculty_id && c.IsSelected.Equals(IsSelected.Selected.ToString()))
                                .Select(c => c.contribution_id)
                                .Count(),

                            NumberContributionRejected = _context.Contributions
                                .Where(c => c.contribution_academic_years_id == getMaxAcademic
                                    && c.users.user_faculty_id == f.faculty_id && c.IsSelected.Equals(IsSelected.Unselected.ToString()))
                                .Select(c => c.contribution_id)
                                .Count(),

                            RateContributionApproved = 0,

                            RateContributionRejected = 0,

                            academic = _context.Academic_Years
                                .Where(a => a.academic_year_id == getMaxAcademic)
                                .Select(a => a.academic_year_title)
                                .FirstOrDefault()

                        }).FirstOrDefault();

                        st.Add(statisticalDataContributions);
                    }                   
                }
            }

            return Ok(st);
        }
        //end

        [HttpGet("statistical_approve_reject_chart")]
        public async Task<IActionResult> statistical_approve_reject_chart(int academic_id)
        {
            var getMaxAcademic = _context.Academic_Years.Max(a => a.academic_year_id);

            var faculty = _context.Faculties
                .Where(f => !f.faculty_name.Contains("None"))
                .Select(f => new
                {
                    f.faculty_id,
                    f.faculty_name
                })
                .ToList();

            var roleID = _context.Roles.Where(r => r.role_name.Equals("Coordinator")).Select(r => r.role_id).FirstOrDefault();


            List<object> st = new List<object>();

            if (academic_id != 0)
            {
                foreach (var i in faculty)
                {
                    var TotalContributor = _context.Users
                            .Include(u => u.role)
                            .Include(u => u.Contributions)
                            .Where(u => u.user_faculty_id == i.faculty_id && u.role.role_name.Equals("Student"))
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
                                && c.users.user_faculty_id == i.faculty_id && c.IsSelected.Equals(IsSelected.Selected.ToString()))
                            .Select(c => c.contribution_id)
                            .Count(),

                        NumberContributionRejected = _context.Contributions
                            .Where(c => c.contribution_academic_years_id == academic_id
                                && c.users.user_faculty_id == i.faculty_id && c.IsSelected.Equals(IsSelected.Unselected.ToString()))
                            .Select(c => c.contribution_id)
                            .Count(),

                        academic = _context.Academic_Years
                            .Where(a => a.academic_year_id == academic_id)
                            .Select(a => a.academic_year_title)
                            .FirstOrDefault()

                    }).FirstOrDefault();

                    st.Add(statisticalDataContributions);
                }
            }
            else
            {
                foreach (var i in faculty)
                {
                    var TotalContributor = _context.Users
                            .Include(u => u.role)
                            .Where(u => u.user_faculty_id == i.faculty_id && u.role.role_name.Equals("Student"))
                            .Select(u => u.user_id)
                            .Count();

                    var TotalContribution = (
                            from c in _context.Contributions
                            join u in _context.Users on c.contribution_user_id equals u.user_id
                            join f in _context.Faculties on u.user_faculty_id equals f.faculty_id
                            where c.contribution_academic_years_id == getMaxAcademic && f.faculty_id == i.faculty_id
                            select c.contribution_id).Count();

                    var statisticalDataContributions = _context.Contributions
                    .Where(c => c.contribution_academic_years_id == getMaxAcademic)
                    .Select(c => new
                    {
                        faculty_name = i.faculty_name,

                        NumberContributor = TotalContributor,

                        NumberContribution = TotalContribution,

                        NumberContributionApproved = _context.Contributions
                            .Where(c => c.contribution_academic_years_id == getMaxAcademic
                                && c.users.user_faculty_id == i.faculty_id && c.IsSelected.Equals(IsSelected.Selected.ToString()))
                            .Select(c => c.contribution_id)
                            .Count(),

                        NumberContributionRejected = _context.Contributions
                            .Where(c => c.contribution_academic_years_id == getMaxAcademic
                                && c.users.user_faculty_id == i.faculty_id && c.IsSelected.Equals(IsSelected.Unselected.ToString()))
                            .Select(c => c.contribution_id)
                            .Count(),

                        academic = _context.Academic_Years
                            .Where(a => a.academic_year_id == getMaxAcademic)
                            .Select(a => a.academic_year_title)
                            .FirstOrDefault()

                    }).FirstOrDefault();

                    st.Add(statisticalDataContributions);
                }
            }

            return Ok(st);
        }

        //[HttpGet("work-duration")]
        //public async Task<IActionResult> workDuration()
        //{
        //    var dataStatistic = await _context.Users
        //        .Select(u => new {
        //            user_id = u.user_id,
        //            user_username = u.user_username,
        //            user_email = u.user_email,
        //            user_role = u.role.role_name,
        //            total_work_duration = u.total_work_duration,
        //            average_work_duration = u.Page_Views.Where(p => p.page_view_user_id == u.user_id)
        //                                        .Select(p => p.time_stamp).FirstOrDefault() == DateTime.Now
        //                                        ? u.total_work_duration : (int)(u.total_work_duration / ((int)(DateTime.Now - u.Page_Views.Where(p => p.page_view_user_id == u.user_id)
        //                                        .Select(p => p.time_stamp).FirstOrDefault()).TotalSeconds))
        //        } ).ToListAsync();

        //    return Ok(dataStatistic);
        //}

        [HttpGet("view-access-page-browser")]
        public async Task<IActionResult> viewAccessPageBrowser()
        {

            var getAllPage = await _context.Page_Views
                .GroupBy(p => p.page_view_name)
                .Select(p => new { p.Key }).ToListAsync();

            if (getAllPage == null) { return BadRequest(new { Message = "Page is null" }); }

            List<Object> dataStatistic = new List<Object>();

            foreach (var page in getAllPage)
            {
                var data = await _context.Page_Views
                    .Where(p => p.page_view_name == page.ToString())
                    .Select(p => new
                    {
                        page_view_name = page.ToString(),
                        browsers = p.browser_name.ToList(),
                        role = _context.Users.Where(u => u.user_id == p.page_view_user_id)
                                .GroupBy(u => u.role.role_name)
                                .Select(u => new
                                {
                                    role_name = u.Key.ToString()
                                }),
                        total_access = p.Count(),
                        total_access_time = p.Sum(),
                        average_access = Math.Round((double)p.Sum() / p.Count()),
                        average_access_date = Math.Round((double)p.Sum() /
                                                (double)(DateTime.Now - _context.Page_Views.Where(p => p.page_view_name == page.ToString())
                                                    .Select(p => p.time_stamp).FirstOrDefault()).TotalSeconds)
                    })
                    .FirstOrDefaultAsync();
                dataStatistic.Add(data);
            }

            return Ok(dataStatistic);
        }

        [HttpGet("work-duration")]
        public async Task<IActionResult> WorkDuration(int user_role_id)
        {
            const double HoursInDay = 24.0;

            IQueryable<Users> query = _context.Users;

            // Filter users by role if user_role_id is provided
            if (user_role_id != 0)
            {
                query = query.Where(u => u.user_role_id == user_role_id);
            }

            var dataStatistic = await _context.Users
                .Select(u => new
                {
                    user_id = u.user_id,
                    user_username = u.user_username,
                    user_email = u.user_email,
                    user_role = u.role.role_name,
                    total_work_duration = Math.Round((double) u.total_work_duration/3600, 4),
                    average_work_duration = Math.Round((double)u.Page_Views
                .Where(p => p.page_view_user_id == u.user_id && p.time_stamp.Date == DateTime.Today)
                .Sum(p => p.total_time_access / 60) / HoursInDay, 4) 
                })
                .ToListAsync();

            // Check if dataStatistic is empty
            if (dataStatistic == null || !dataStatistic.Any())
            {
                return BadRequest(new { Message = "Data is empty" });
            }

            return Ok(dataStatistic);
        }

        [HttpGet("chart-guest")]
        public async Task<IActionResult> chartGuest(string username)
        {
            if (username == null) { return BadRequest(new { Message = "Data provided is null" }); }

            var getFaculty = _context.Users.Where(u => u.user_username == username).Select(u => u.user_faculty_id).FirstOrDefault();

            var getAllAcademic = _context.Academic_Years.ToList();

            List<Object> dataStatistic = new List<Object>();

            foreach (var academic in getAllAcademic)
            {
                var data = _context.Contributions
                    .Where(c => c.users.user_faculty_id == getFaculty)
                    .Select(c => new
                    {
                        label = academic.academic_year_FinalClosureDate.ToString("yyyy-MM-dd"),
                        totalArticle = _context.Contributions
                                        .Where(c => c.users.user_faculty_id == getFaculty
                                            && c.contribution_academic_years_id == academic.academic_year_id)
                                        .Count(),
                        totalContributor = _context.Contributions
                                        .Where(c => c.users.user_faculty_id == getFaculty
                                            && c.contribution_academic_years_id == academic.academic_year_id)
                                        .GroupBy(c => c.contribution_user_id)
                                        .Count(),
                    }).FirstOrDefault();

                dataStatistic.Add(data);
            }

            return Ok(dataStatistic);
        }
        [HttpGet("chart-admin")]
        public async Task<IActionResult> chartAdmin()
        {
            var faculties = _context.Faculties
                        .Where(fa => fa.faculty_name != "Admin_Account" && fa.faculty_name != "Manager_Account").ToList();

            List<Object> dataStatistic = new List<Object>();

            double totalArticle = await _context.Contributions.CountAsync();

            foreach (var facultie in faculties)
            {
                var data = _context.Contributions
                    .Select(c => new
                    {
                        facultyName = facultie.faculty_name,
                        contributors = _context.Contributions
                                                    .Where(c => c.users.faculties.faculty_id == facultie.faculty_id)
                                                    .GroupBy(c => c.contribution_user_id).Count(),
                        articles = _context.Contributions
                                                    .Where(c => c.users.faculties.faculty_id == facultie.faculty_id).Count(),
                        percenContributor = Math.Round(((double)(_context.Contributions
                                                    .Where(c => c.users.faculties.faculty_id == facultie.faculty_id)
                                                    .GroupBy(c => c.contribution_user_id).Count()) / (double)(_context.Contributions
                                                    .GroupBy(c => c.contribution_user_id).Count()) * 100)),
                        percenArticles = Math.Round((((double)(_context.Contributions
                                                    .Where(c => c.users.faculties.faculty_id == facultie.faculty_id).Count()))
                                                     / (totalArticle)) * 100)
                    }).FirstOrDefault();

                dataStatistic.Add(data);
            }

            return Ok(dataStatistic);
        }
        [HttpGet("page-browser-role-statistic")]
        public async Task<IActionResult> pageBrowserRoleStatistic()
        {

            var data = _context.Page_Views.GroupBy(p => p.browser_name)
                                          .Select(pv => new
                                          {
                                               pageName = "",
                                               browserName = pv.Key,
                                               username = "",
                                               totalVisit = _context.Page_Views.Where(p => p.browser_name == pv.Key).Count(),
                                               averageTime = Math.Round((double)_context.Page_Views.Where(p => p.browser_name == pv.Key).Sum(p => p.total_time_access) /
                                               (double)_context.Page_Views.Where(p => p.browser_name == pv.Key).Count() / 3600, 4),
                                               dailyAverageTime = Math.Round((double)_context.Page_Views.Where(p => p.browser_name == pv.Key).Sum(p => p.total_time_access) /
                                               (((double)(_context.Page_Views.Where(p => p.browser_name == pv.Key).OrderBy(p => p.time_stamp).Select(p => p.time_stamp).Last().Day
                                               - _context.Page_Views.Where(p => p.browser_name == pv.Key).OrderBy(p => p.time_stamp).Select(p => p.time_stamp).FirstOrDefault().Day)
                                               ) == 0 ? 1 : ((double)(_context.Page_Views.Where(p => p.browser_name == pv.Key).OrderBy(p => p.time_stamp).Select(p => p.time_stamp).Last().Day
                                               - _context.Page_Views.Where(p => p.browser_name == pv.Key).OrderBy(p => p.time_stamp).Select(p => p.time_stamp).FirstOrDefault().Day)
                                               )) / 3600, 4),
                                               totalTime = Math.Round((double)_context.Page_Views.Where(p => p.browser_name == pv.Key).Sum(p => p.total_time_access) / 3600, 4)
                                          }).ToList();
            return Ok(data);
        }
        [HttpGet("page-chart")]
        public async Task<IActionResult> pageChart()
        {
            var data = _context.Page_Views
                               .GroupBy(p => p.page_view_name)
                               .Select(pv => new
                               {
                                   pageName = pv.Key.ToString(),
                                   totalVisit = _context.Page_Views.Where(p => p.page_view_name == pv.Key).Count(),
                                   averageTime = Math.Round((double)_context.Page_Views.Where(p => p.page_view_name == pv.Key).Sum(p => p.total_time_access) /
                                   (double)_context.Page_Views.Where(p => p.page_view_name == pv.Key).Count() / 3600, 4),
                                   dailyAverageTime = Math.Round((double)_context.Page_Views.Where(p => p.page_view_name == pv.Key).Sum(p => p.total_time_access) /
                                   (((double)(_context.Page_Views.Where(p => p.page_view_name == pv.Key).OrderBy(p => p.time_stamp).Select(p => p.time_stamp).Last().Day
                                   - _context.Page_Views.Where(p => p.page_view_name == pv.Key).OrderBy(p => p.time_stamp).Select(p => p.time_stamp).FirstOrDefault().Day)
                                   ) == 0 ? 1 : ((double)(_context.Page_Views.Where(p => p.page_view_name == pv.Key).OrderBy(p => p.time_stamp).Select(p => p.time_stamp).Last().Day
                                   - _context.Page_Views.Where(p => p.page_view_name == pv.Key).OrderBy(p => p.time_stamp).Select(p => p.time_stamp).FirstOrDefault().Day)
                                   )) / 3600, 4),
                                   totalTime = Math.Round((double)_context.Page_Views.Where(p => p.page_view_name == pv.Key).Sum(p => p.total_time_access) / 3600, 4)
                               }).ToList();
            return Ok(data);
        }

        [HttpGet("browser-chart")]
        public async Task<IActionResult> browserChart()
        {
            var data = _context.Page_Views
                               .GroupBy(p => p.browser_name)
                               .Select(pv => new
                               {
                                   browserName = pv.Key.ToString(),
                                   totalVisit = _context.Page_Views.Where(p => p.browser_name == pv.Key).Count(),
                                   averageTime = Math.Round((double)_context.Page_Views.Where(p => p.browser_name == pv.Key).Sum(p => p.total_time_access) /
                                   (double)_context.Page_Views.Where(p => p.browser_name == pv.Key).Count() / 3600, 4),
                                   dailyAverageTime = Math.Round((double)_context.Page_Views.Where(p => p.browser_name == pv.Key).Sum(p => p.total_time_access) /
                                   (((double)(_context.Page_Views.Where(p => p.browser_name == pv.Key).OrderBy(p => p.time_stamp).Select(p => p.time_stamp).Last().Day
- _context.Page_Views.Where(p => p.browser_name == pv.Key).OrderBy(p => p.time_stamp).Select(p => p.time_stamp).FirstOrDefault().Day)
                                   ) == 0 ? 1 : ((double)(_context.Page_Views.Where(p => p.browser_name == pv.Key).OrderBy(p => p.time_stamp).Select(p => p.time_stamp).Last().Day
                                   - _context.Page_Views.Where(p => p.browser_name == pv.Key).OrderBy(p => p.time_stamp).Select(p => p.time_stamp).FirstOrDefault().Day)
                                   )) / 3600, 4),
                                   totalTime = Math.Round((double)_context.Page_Views.Where(p => p.browser_name == pv.Key).Sum(p => p.total_time_access) / 3600, 4)
                               }).ToList();
            return Ok(data);
        }

        [HttpGet("role-chart")]
        public async Task<IActionResult> roleChart()
        {
            var data = _context.Page_Views
                               .GroupBy(p => p.users.role.role_name)
                               .Select(pv => new
                               {
                                   roleName = pv.Key.ToString(),
                                   totalVisit = _context.Page_Views.Where(p => p.users.role.role_name == pv.Key).Count(),
                                   averageTime = Math.Round(((double)_context.Page_Views.Where(p => p.users.role.role_name == pv.Key).Sum(p => p.total_time_access) /
                                   (double)_context.Page_Views.Where(p => p.users.role.role_name == pv.Key).Count()) / 3600, 4),
                                   dailyAverageTime = Math.Round(((double)_context.Page_Views.Where(p => p.users.role.role_name == pv.Key).Sum(p => p.total_time_access) /
                                   (((double)(_context.Page_Views.Where(p => p.users.role.role_name == pv.Key).OrderBy(p => p.time_stamp).Select(p => p.time_stamp).Last().Day
                                   - _context.Page_Views.Where(p => p.users.role.role_name == pv.Key).OrderBy(p => p.time_stamp).Select(p => p.time_stamp).FirstOrDefault().Day)
                                   ) == 0 ? 1 : ((double)(_context.Page_Views.Where(p => p.users.role.role_name == pv.Key).OrderBy(p => p.time_stamp).Select(p => p.time_stamp).Last().Day
                                   - _context.Page_Views.Where(p => p.users.role.role_name == pv.Key).OrderBy(p => p.time_stamp).Select(p => p.time_stamp).FirstOrDefault().Day)
                                   ))) / 3600, 4),
                                   totalTime = Math.Round((double)_context.Page_Views.Where(p => p.users.role.role_name == pv.Key).Sum(p => p.total_time_access) / 3600, 4)
                               }).ToList();
            return Ok(data);
        }
    }
}
