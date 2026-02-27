using WebQuanLiKhoaHocApi.Entities;
using Microsoft.EntityFrameworkCore;
using WebQuanLiKhoaHocApi.Interfaces.HocVien;
using WebQuanLiKhoaHocApi.Dtos;
namespace WebQuanLiKhoaHocApi.Services.HocVien
{
    public class HoSoHocVienService : IHoSoHocVien
    {
        private readonly UniversityDBContext context;
        public HoSoHocVienService(UniversityDBContext context)
        {
            this.context = context;
        }
        public async Task<HocVien_HoSoCaNhan?> LayHoSoHocvien(string MaHocvien)
        {
            var query = from u in context.Users
                        join s in context.Students on u.UserId equals s.StudentId // Giả sử UserId khớp với StudentId
                        where u.Username == MaHocvien // So sánh với Username "student1"
                        select new HocVien_HoSoCaNhan
                        {
                            MaHocVien = s.StudentNumber, // Trả về "SV001"
                            HoVaTen = s.FullName,
                            NgaySinh = s.DateOfBirth,
                            NganhHoc = s.Faculty,
                            KhoaNhapHoc = s.Year
                        };

            return await query.FirstOrDefaultAsync();
        }
    }
}
