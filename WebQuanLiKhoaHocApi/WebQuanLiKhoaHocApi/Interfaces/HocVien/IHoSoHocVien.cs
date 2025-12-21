using WebQuanLiKhoaHocApi.Dtos;

namespace WebQuanLiKhoaHocApi.Interfaces.HocVien
{
    public interface IHoSoHocVien
    {
        Task<HocVien_HoSoCaNhan?> LayHoSoHocvien(string MaHocvien);
        
    }
}
