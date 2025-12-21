using Microsoft.AspNetCore.Mvc;
using WebQuanLiKhoaHoc_MVC.Models.HocVien;
using WebQuanLiKhoaHoc_MVC.Service;
// 1. Thêm namespace chứa Model Bài tập
using WebQuanLiKhoaHoc_MVC.Models.HocVien;

namespace WebQuanLiKhoaHoc_MVC.Controllers.HocVien
{
    public class XemDiemController : Controller
    {
        // Khai báo service cũ
        private readonly HocVien_XemDiemService hocVien_XemDiemTBService;

        // 2. Khai báo service MỚI (Bài tập)
        private readonly BaiTapService _baiTapService;

        // Cập nhật Constructor để nhận cả 2 Service
        public XemDiemController(HocVien_XemDiemService hocVien_XemDiemTBService, BaiTapService baiTapService)
        {
            this.hocVien_XemDiemTBService = hocVien_XemDiemTBService;
            this._baiTapService = baiTapService; // Gán giá trị
        }

        public async Task<IActionResult> XemDiemTrungBinh(string MaHocVien)
        {
            string viewPath = "~/Views/Student/XemDiemTb.cshtml";

            if (string.IsNullOrEmpty(MaHocVien))
            {
                return View(viewPath, new List<HocVien_XemDiemTBModels>());
            }

            var xemDiemTrungBinhs = await hocVien_XemDiemTBService.XemDiemTrungBinh(MaHocVien);
            ViewBag.CurrentFilter = MaHocVien;

            return View(viewPath, xemDiemTrungBinhs);
        }

        public async Task<IActionResult> XemDiemMon(string MaHocVien, string MaHocPhan)
        {
            string viewPath = "~/Views/Student/XemDiemChiTiet.cshtml";

            if (string.IsNullOrEmpty(MaHocVien))
            {
                return View(viewPath, new List<HocVien_XemDiemMonModels>());
            }

            // Lấy bảng điểm chi tiết (Code cũ)
            var xemDiemMon = await hocVien_XemDiemTBService.XemDiemMonHoc(MaHocVien, MaHocPhan);

            // 3. [MỚI] Gọi API lấy danh sách bài tập chưa nộp
            // Lưu ý: Biến 'MaHocPhan' ở đây đóng vai trò là Mã Lớp (ClassCode) để tìm bài tập
            var listBaiTap = await _baiTapService.LayBaiTapChuaNop(MaHocVien, MaHocPhan);

            // Đẩy dữ liệu sang View bằng ViewBag
            ViewBag.ListBaiTapChuaNop = listBaiTap;

            // Các ViewBag cũ
            ViewBag.CurrentFilter = MaHocVien;
            ViewBag.MaHocVien = MaHocVien; // Đảm bảo view có biến này để dùng cho nút quay lại

            return View(viewPath, xemDiemMon);
        }
    }
}