using Microsoft.EntityFrameworkCore;
using WebQuanLiKhoaHocApi.Dtos;
using WebQuanLiKhoaHocApi.Entities;

namespace WebQuanLiKhoaHocApi.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly UniversityDBContext _context;
        public ScheduleService(UniversityDBContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<HocVienScheduleDto>> GetStudentScheduleAsync(int studentId)
        {
            // Logic truy vấn sử dụng các mối quan hệ có sẵn:
            // Student -> Registration -> Class -> ClassSchedule
            var schedule = await _context.Registrations
                .Where(r => r.StudentId == studentId)
                .Select(r => r.Class)

                // Lấy tất cả các buổi học (ClassSchedule) của Class đã đăng ký
                .SelectMany(c => c.ClassSchedules, (c, cs) => new { Class = c, Schedule = cs })

                // Ánh xạ (Map) kết quả sang DTO
                .Select(x => new HocVienScheduleDto
                {
                    // Thông tin Buổi học (ClassSchedule)
                    ScheduleId = x.Schedule.ScheduleId,
                    NgayTrongTuan = x.Schedule.DayOfWeek,
                    ThoiGianBatDau = x.Schedule.StartTime,
                    ThoiGianKetThuc = x.Schedule.EndTime,
                    PhongHoc = x.Schedule.Room,

                    // Thông tin Lớp học (Class) và Khóa học (Course)
                    MaLopHoc = x.Class.ClassCode,
                    HocKy = x.Class.Semester,
                    MaKhoaHoc = x.Class.Course.CourseCode,
                    TenKhoaHoc = x.Class.Course.CourseName,
                    Credit = x.Class.Course.Credit,

                    // Thông tin Giảng viên (Lecturer)
                    // Lưu ý: Cần đảm bảo Class.Lecturer và Class.Lecturer.FullName không bị null
                    TenGiangVien = x.Class.Lecturer != null ? x.Class.Lecturer.FullName : "Chưa cập nhật"
                })
                .OrderBy(dto => dto.NgayTrongTuan)
                .ThenBy(dto => dto.ThoiGianBatDau)
                .ToListAsync();

            return schedule;
        }
    }
}
