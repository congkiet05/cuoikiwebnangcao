using Microsoft.EntityFrameworkCore;
using WebQuanLiKhoaHocApi.Dtos;
using WebQuanLiKhoaHocApi.Entities;
using WebQuanLiKhoaHocApi.Interfaces;
using WebQuanLiKhoaHocApi.Interfaces.HocVien;

namespace WebQuanLiKhoaHocApi.Services.BaiTap
{
    public class HocVien_BaiTapService : IHocVien_NopBaiTap
    {
        private readonly UniversityDBContext _context; // Đảm bảo tên DbContext đúng với dự án của bạn

        public HocVien_BaiTapService(UniversityDBContext context)
        {
            _context = context;
        }

        public async Task<List<HocVien_DanhSachNopBaiTap>> LayBaiTapChuaNop(string studentNumber, string classCode)
        {
            
            var query = from a in _context.Assignments
                        join c in _context.Classes on a.ClassId equals c.ClassId
                        join co in _context.Courses on c.CourseId equals co.CourseId

                        // JOIN: Tìm lớp mà SV đang thực sự học (Registration)
                        join r in _context.Registrations on c.ClassId equals r.ClassId
                        join stu in _context.Students on r.StudentId equals stu.StudentId

                        // JOIN Submission
                        join s in _context.Submissions
                             on new { a.AssignmentId, StudentId = stu.StudentId }
                             equals new { s.AssignmentId, s.StudentId } into subs
                        from sub in subs.DefaultIfEmpty()

                        where stu.StudentNumber == studentNumber
                           // SỬA ĐỔI QUAN TRỌNG TẠI ĐÂY:
                           // So sánh Mã Môn (CourseCode) thay vì Mã Lớp (ClassCode)
                           // Vì "CT101" (Môn) sẽ khớp, còn "CT101_B" (Lớp) thì không khớp với input "CT101"
                           && co.CourseCode == classCode
                           && sub == null // Chưa nộp

                        select new HocVien_DanhSachNopBaiTap
                        {
                            MaMonHoc = co.CourseCode,
                            TenMonHoc = co.CourseName,
                            AssignmentId = a.AssignmentId,
                            TieuDe = a.Title,
                            HanNop = a.DueDate,
                            TrangThai = (a.DueDate.HasValue && a.DueDate.Value < DateTime.Now) ? "Quá hạn" : "Đang mở"
                        };

            return await query.ToListAsync();
        }
        public async Task<BaiTapChiTietDto> LayChiTietBaiTap(int id)
        {
            var data = await _context.Assignments
                .Where(a => a.AssignmentId == id)
                .Select(a => new BaiTapChiTietDto // Map sang DTO
                {
                    AssignmentId = a.AssignmentId,
                    Title = a.Title,
                    Description = a.Description,
                    DueDate = a.DueDate,
                    TenMon = a.Class.Course.CourseName
                })
                .FirstOrDefaultAsync();

            return data; // Trả về null nếu không tìm thấy
        }
        public async Task<bool> NopBai(HocVien_NopBaiDto model)
        {
            // 1. Tìm StudentId
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentNumber == model.StudentNumber);
            if (student == null) return false;

            // 2. Tạo đối tượng Submission (Khớp với Entity bạn gửi)
            var submission = new Submission
            {
                AssignmentId = model.AssignmentId,
                StudentId = student.StudentId,
                FileUrl = model.FileUrl,      // Đã sửa thành FileUrl
                SubmittedAt = DateTime.Now,   // Thời gian nộp
                Grade = null                  // Mới nộp thì chưa có điểm
                                              // Lưu ý: Không gán Notes vì Entity không có trường này
            };

            // 3. Lưu vào DB
            _context.Submissions.Add(submission);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}