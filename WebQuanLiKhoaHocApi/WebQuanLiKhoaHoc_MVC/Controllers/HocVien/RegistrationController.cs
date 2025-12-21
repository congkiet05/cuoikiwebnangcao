using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebQuanLiKhoaHoc_MVC.Models.HocVien;
using WebQuanLiKhoaHocApi.Entities;
using System.Security.Claims;

namespace WebQuanLiKhoaHoc_MVC.Controllers.HocVien
{
    public class RegistrationController : Controller
    {
        private readonly UniversityDBContext _context;

        public RegistrationController(UniversityDBContext context) => _context = context;

        // Hiển thị danh sách lớp học
        public async Task<IActionResult> Registration()
        {
            var classes = await _context.Classes
                .Include(c => c.Course).Include(c => c.Lecturer).Include(c => c.ClassSchedules)
                .Select(c => new ClassEnrollmentViewModel
                {
                    ClassId = c.ClassId,
                    ClassCode = c.ClassCode,
                    CourseName = c.Course.CourseName,
                    LecturerName = c.Lecturer.FullName,
                    Capacity = c.Capacity,
                    EnrolledCount = _context.Registrations.Count(r => r.ClassId == c.ClassId),
                    Schedules = c.ClassSchedules.ToList()
                }).ToListAsync();
            return View(classes);
        }

        // Xử lý đăng ký môn học
        [HttpPost]
        public async Task<IActionResult> Enroll(int classId)
        {
            // 1. Lấy ID sinh viên (Giả sử ID=1 nếu chưa Login)
            int studentId = 1;

            // 2. Kiểm tra sĩ số
            var targetClass = await _context.Classes.Include(c => c.ClassSchedules).FirstOrDefaultAsync(x => x.ClassId == classId);
            int current = await _context.Registrations.CountAsync(r => r.ClassId == classId);
            if (current >= targetClass.Capacity)
            {
                TempData["Error"] = "Lớp đã đầy!";
                return RedirectToAction("Registration");
            }

            // 3. Kiểm tra trùng lịch (Logic LINQ nâng cao)
            var mySchedules = await _context.Registrations
                .Where(r => r.StudentId == studentId)
                .SelectMany(r => r.Class.ClassSchedules).ToListAsync();

            foreach (var newS in targetClass.ClassSchedules)
            {
                if (mySchedules.Any(oldS => oldS.DayOfWeek == newS.DayOfWeek &&
                    newS.StartTime < oldS.EndTime && newS.EndTime > oldS.StartTime))
                {
                    TempData["Error"] = $"Trùng lịch Thứ {newS.DayOfWeek} ({newS.StartTime}-{newS.EndTime})";
                    return RedirectToAction("Registration");
                }
            }

            // 4. Lưu đăng ký
            _context.Registrations.Add(new WebQuanLiKhoaHocApi.Entities.Registration
            {
                StudentId = studentId,
                ClassId = classId,
                RegisteredAt = DateTime.Now,
                Status = "Registered"
            });
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đăng ký thành công!";
            return RedirectToAction("Registration");
        }
    }
}