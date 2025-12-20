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
            var hocVien = await context.Students
                .Where(hv => hv.StudentNumber == MaHocvien)
                .Select(hv => new HocVien_HoSoCaNhan()
                {
                    MaHocVien = hv.StudentNumber,
                    HoVaTen = hv.FullName,
                    NgaySinh = hv.DateOfBirth,
                    NganhHoc = hv.Faculty,
                    KhoaNhapHoc = hv.Year
                })
                .FirstOrDefaultAsync();
            return hocVien;
        }
    }
}
