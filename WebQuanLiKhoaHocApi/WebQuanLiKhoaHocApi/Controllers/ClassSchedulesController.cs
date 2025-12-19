using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebQuanLiKhoaHocApi.Entities;

namespace WebQuanLiKhoaHocApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassSchedulesController : ControllerBase
    {
        private readonly UniversityDBContext _context;

        public ClassSchedulesController(UniversityDBContext context)
        {
            _context = context;
        }

        // 1. Lấy toàn bộ lịch dạy của một giảng viên
        // GET: api/ClassSchedules/Lecturer/2
        [HttpGet("Lecturer/{lecturerId}")]
        public async Task<IActionResult> GetSchedulesByLecturer(int lecturerId)
        {
            var schedules = await _context.ClassSchedules
                .Include(s => s.Class)
                    .ThenInclude(c => c.Course)
                .Where(s => s.Class.LecturerId == lecturerId)
                .Select(s => new
                {
                    s.ScheduleId,
                    s.DayOfWeek,
                    StartTime = s.StartTime.ToString("HH:mm"), // Chuyển TimeOnly sang string cho JSON
                    EndTime = s.EndTime.ToString("HH:mm"),
                    s.Room,
                    CourseName = s.Class.Course.CourseName,
                    CourseCode = s.Class.Course.CourseCode,
                    s.Class.ClassCode
                })
                .OrderBy(s => s.DayOfWeek)
                .ThenBy(s => s.StartTime)
                .ToListAsync();

            return Ok(schedules);
        }

        // 2. Lấy lịch dạy của giảng viên trong ngày hôm nay
        // GET: api/ClassSchedules/Lecturer/2/Today
        [HttpGet("Lecturer/{lecturerId}/Today")]
        public async Task<IActionResult> GetTodaySchedules(int lecturerId)
        {
            var dayNow = DateTime.Now.DayOfWeek;
            byte dayOfWeekDb = dayNow switch
            {
                DayOfWeek.Sunday => 7,
                _ => (byte)dayNow
            };

            var todaySchedules = await _context.ClassSchedules
                .Include(s => s.Class)
                    .ThenInclude(c => c.Course)
                .Where(s => s.Class.LecturerId == lecturerId && s.DayOfWeek == dayOfWeekDb)
                // ✅ Sắp xếp theo giá trị TimeOnly gốc (EF Core dịch được cái này)
                .OrderBy(s => s.StartTime)
                .Select(s => new
                {
                    s.ScheduleId,
                    // Định dạng chuỗi ở đây để trả về cho Client
                    StartTime = s.StartTime.ToString("HH:mm"),
                    EndTime = s.EndTime.ToString("HH:mm"),
                    s.Room,
                    CourseName = s.Class.Course.CourseName,
                    CourseCode = s.Class.Course.CourseCode
                })
                .ToListAsync();

            return Ok(todaySchedules);
        }
    }
}