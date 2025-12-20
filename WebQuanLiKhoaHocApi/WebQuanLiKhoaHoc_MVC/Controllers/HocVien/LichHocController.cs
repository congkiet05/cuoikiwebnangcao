using Microsoft.AspNetCore.Mvc;
using WebQuanLiKhoaHoc_MVC.Models.HocVien;
using WebQuanLiKhoaHoc_MVC.Service;

namespace WebQuanLiKhoaHoc_MVC.Controllers.HocVien
{
    public class LichHocController : Controller
    {
        private readonly HocVienLichHoc _hocVienLichHoc;
        public LichHocController(HocVienLichHoc hocVienLichHoc)
        {
            _hocVienLichHoc = hocVienLichHoc;
        }
        public async Task<IActionResult> LichHoc(string MaHocVien)
        {
            string viewPath = "~/Views/Student/LichHoc.cshtml";

            if (string.IsNullOrEmpty(MaHocVien))
            {
                // SỬA LẠI: Thêm viewPath vào đây
                return View(viewPath, new List<LichHocModel>());
            }

            var lichHoc = await _hocVienLichHoc.XemLichHoc(MaHocVien);
            ViewBag.CurrentFilter = MaHocVien;

            // Dòng dưới này bạn đã làm đúng rồi
            return View(viewPath, lichHoc);
        }
    }
}
