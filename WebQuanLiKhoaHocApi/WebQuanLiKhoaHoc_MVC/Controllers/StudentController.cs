using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Formats.Asn1;
using System.Security.Claims;
using WebQuanLiKhoaHoc_MVC.Interface;
using WebQuanLiKhoaHoc_MVC.Models;
using WebQuanLiKhoaHocApi.Entities;
namespace WebQuanLiKhoaHoc_MVC.Controllers
{
    public class StudentController : Controller
    {
        private readonly UniversityDBContext _context;
        public IActionResult Dashboard()
        {
            return View(); // -> Views/Student/Dashboard.cshtml
        }
        public StudentController(UniversityDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> StudentSchedule()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            int studentId = 1;
            if (!string.IsNullOrEmpty(idClaim) && int.TryParse(idClaim, out var parsedId))
            {
                studentId = parsedId;
            }

            var schedules = await (from reg in _context.Registrations
                                   where reg.StudentId == studentId
                                   join cls in _context.Classes on reg.ClassId equals cls.ClassId
                                   join crs in _context.Courses on cls.CourseId equals crs.CourseId
                                   join lec in _context.Lecturers on cls.LecturerId equals lec.LecturerId into lecj
                                   from lec in lecj.DefaultIfEmpty()
                                   join sch in _context.ClassSchedules on cls.ClassId equals sch.ClassId
                                   select new StudentScheduleViewModel
                                   {
                                       MaKhoaHoc = crs.CourseCode,
                                       TenKhoaHoc = crs.CourseName,
                                       Credit = crs.Credit,
                                       MaLopHoc = cls.ClassCode,
                                       HocKy = cls.Semester,
                                       TenGiangVien = lec != null ? lec.FullName : string.Empty,
                                       ScheduleId = sch.ScheduleId,
                                       NgayTrongTuan = sch.DayOfWeek,
                                       ThoiGianBatDau = sch.StartTime,
                                       ThoiGianKetThuc = sch.EndTime,
                                       PhongHoc = sch.Room
                                   }).ToListAsync();

            return View(schedules);
        }
    }
}
