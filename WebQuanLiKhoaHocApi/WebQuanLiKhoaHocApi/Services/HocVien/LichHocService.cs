using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebQuanLiKhoaHocApi.Dtos;
using WebQuanLiKhoaHocApi.Entities;
namespace WebQuanLiKhoaHocApi.Interfaces.HocVien
{
    public class LichHocService : ILichHoc
    {
        private readonly UniversityDBContext context;
        public LichHocService(UniversityDBContext _context) {
            this.context = _context;
        }
        public async Task<List<HocVien_LichHoc>> XemLichHoc(string maSinhVien)
        {
            var query = from s in context.Students
                        join r in context.Registrations on s.StudentId equals r.StudentId
                        join c in context.Classes on r.ClassId equals c.ClassId
                        join co in context.Courses on c.CourseId equals co.CourseId
                        join cs in context.ClassSchedules on c.ClassId equals cs.ClassId
                        // Left Join Giảng viên (đề phòng chưa có giảng viên)
                        join l in context.Lecturers on c.LecturerId equals l.LecturerId into lGroup
                        from l in lGroup.DefaultIfEmpty()

                        where s.StudentNumber == maSinhVien
                        // Có thể thêm điều kiện lọc theo Học kì hiện tại nếu cần
                        // && c.Semester == "2025 Spring" 

                        select new HocVien_LichHoc
                        {
                            MaSinhVien = s.StudentNumber,
                            TenHocVien = s.FullName,
                            MaLopHoc = c.ClassCode,
                            TenMonHoc = co.CourseName,
                            Thu = cs.DayOfWeek,
                            GioBatDau = cs.StartTime.ToString(@"hh\:mm"),
                            GioKetThuc = cs.EndTime.ToString(@"hh\:mm"),
                            PhongHoc = cs.Room,
                            TenGiangVien = l.FullName ,
                            Hocky = c.Semester
                        };
            return await query.ToListAsync();
        }
    }
}
