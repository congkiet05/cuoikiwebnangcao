using WebQuanLiKhoaHocApi.Dtos;
using WebQuanLiKhoaHocApi.Entities;
using WebQuanLiKhoaHocApi.Interfaces.HocVien;
using Microsoft.EntityFrameworkCore;
namespace WebQuanLiKhoaHocApi.Services.HocVien
{
    public class HocVien_XemDiemService : IXemDiem
    {
        private readonly UniversityDBContext _context;
        public HocVien_XemDiemService(UniversityDBContext context)
        {
            _context = context;
        }
        public async Task<List<HocVien_XemDiemTrungBinh>> XemDiemTrungBinhTatCaCacMon(string maHocVien)
        {
            var query = from s in _context.Students
                        join sub in _context.Submissions on s.StudentId equals sub.StudentId
                        join a in _context.Assignments on sub.AssignmentId equals a.AssignmentId
                        join c in _context.Classes on a.ClassId equals c.ClassId
                        join co in _context.Courses on c.CourseId equals co.CourseId

                        // Xử lý LEFT JOIN Lecturer (vì Lecturer có thể null)
                        join l in _context.Lecturers on c.LecturerId equals l.LecturerId into lGroup
                        from l in lGroup.DefaultIfEmpty()

                        where s.StudentNumber == maHocVien

                        // Group By theo các trường bạn muốn hiển thị
                        group sub by new
                        {
                            s.StudentNumber,
                            s.FullName,
                            s.Faculty,
                            co.CourseCode,
                            co.CourseName,
                            LecturerName = l.FullName, // Có thể null nếu Left Join không thấy
                            c.Semester
                        } into g

                        // Select ra kết quả cuối cùng
                        select new HocVien_XemDiemTrungBinh
                        {
                            MaHocVien = g.Key.StudentNumber,
                            TenHocVien = g.Key.FullName,
                            NganhHoc = g.Key.Faculty,
                            MaHocPhan = g.Key.CourseCode,
                            TenHocPhan = g.Key.CourseName,
                            // Xử lý null cho tên giảng viên
                            GiangVien = g.Key.LecturerName ?? "Chưa phân công",
                            HocKi = g.Key.Semester,

                            // Tính điểm trung bình
                            DiemTrungBinh = g.Average(x => x.Grade)
                        };
            return await query.ToListAsync();
        }
        public async Task<List<HocVien_XemDiemMon>> XemDiemMonHoc(string MaSinhVien, string MaKhoaHoc)
        {
            var query = from s in _context.Students
                        join sub in _context.Submissions on s.StudentId equals sub.StudentId
                        join a in _context.Assignments on sub.AssignmentId equals a.AssignmentId
                        join c in _context.Classes on a.ClassId equals c.ClassId
                        join co in _context.Courses on c.CourseId equals co.CourseId


                        where s.StudentNumber == MaSinhVien && co.CourseCode == MaKhoaHoc

                        // WHERE co.CourseCode = 'CT101' 
                        // LƯU Ý: SQL của bạn đang lọc theo Mã Môn. 
                        // Nếu bạn muốn lọc theo Học Kỳ (như tên hàm), hãy dùng dòng dưới đây (giả sử bảng Class có cột Semester):
                        // && c.Semester == HocKi 

                        // ORDER BY sub.SubmittedAt
                        orderby sub.SubmittedAt

                        // SELECT ...
                        select new HocVien_XemDiemMon
                        {
                            StudentNumber = s.StudentNumber,
                            FullName = s.FullName,
                            ClassCode = c.ClassCode,
                            CourseCode = co.CourseCode,
                            CourseName = co.CourseName,
                            AssignmentTitle = a.Title,
                            CreatedAt = a.CreatedAt,
                            DueDate = a.DueDate,
                            Grade = sub.Grade,
                            SubmittedAt = sub.SubmittedAt
                        };

            // Thực thi query và trả về list bất đồng bộ
            return await query.ToListAsync();
        }
    }
}