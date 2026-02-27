using WebQuanLiKhoaHocApi.Dtos;

namespace WebQuanLiKhoaHocApi.Interfaces.HocVien
{
    public interface IHocVien_NopBaiTap
    {
        Task<List<HocVien_DanhSachNopBaiTap>> LayBaiTapChuaNop(string studentNumber, string classCode);
        Task<BaiTapChiTietDto> LayChiTietBaiTap(int id);
        Task<bool> NopBai(HocVien_NopBaiDto model);
    }
}
