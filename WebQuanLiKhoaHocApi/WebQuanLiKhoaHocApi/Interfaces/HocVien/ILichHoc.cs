using WebQuanLiKhoaHocApi.Dtos;

namespace WebQuanLiKhoaHocApi.Interfaces.HocVien
{
    public interface ILichHoc
    {
        Task<List<HocVien_LichHoc>> XemLichHoc(string maSinhVien);

    }
}
