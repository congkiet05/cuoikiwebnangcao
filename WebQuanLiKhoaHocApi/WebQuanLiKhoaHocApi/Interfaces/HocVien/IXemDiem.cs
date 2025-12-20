using WebQuanLiKhoaHocApi.Dtos;

namespace WebQuanLiKhoaHocApi.Interfaces.HocVien
{
    public interface IXemDiem
    {
        public Task<List<HocVien_XemDiemTrungBinh>> XemDiemTrungBinhTatCaCacMon(string MaSinhVien);
    }
}
