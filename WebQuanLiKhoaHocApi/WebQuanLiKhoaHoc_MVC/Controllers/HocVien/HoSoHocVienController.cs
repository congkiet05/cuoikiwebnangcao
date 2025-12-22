using Microsoft.AspNetCore.Mvc;
using WebQuanLiKhoaHoc_MVC.Service;
using WebQuanLiKhoaHocApi.Entities;

namespace WebQuanLiKhoaHoc_MVC.Controllers.HocVien
{
    
    public class HoSoHocVienController : Controller
    {
        private readonly HoSoHocVienService hoSoHocVienService;

        public HoSoHocVienController(HoSoHocVienService hoSoHocVienService)
        {
            this.hoSoHocVienService = hoSoHocVienService;
        }
        public async Task<IActionResult> HoSoHocVien (string MaHocVien)
        {
            var loggedInUser = User.Identity?.Name;

            // 2. Logic xác định mã cần xem:
            // Nếu MaHocVien truyền vào trống, hãy dùng mã của người đang đăng nhập
            string maDeTruyVan = string.IsNullOrEmpty(MaHocVien) ? loggedInUser : MaHocVien;
            System.Diagnostics.Debug.WriteLine("=== MA TRUY VAN: " + maDeTruyVan);
            if (string.IsNullOrEmpty(maDeTruyVan))
            {
                return BadRequest("Không thể xác định mã học viên.");
            }

            // 3. Gọi service lấy dữ liệu
            var hocVien = await hoSoHocVienService.LayHoSoHocVien(maDeTruyVan);

            if (hocVien == null)
            {
                // Thay vì trả về trang Error chung chung, có thể trả về view kèm thông báo
                ViewBag.ErrorMessage = $"Không tìm thấy thông tin cho mã: {maDeTruyVan}";
                return View("~/Views/Student/HoSoHocVien.cshtml", null);
            }

            return View("~/Views/Student/HoSoHocVien.cshtml", hocVien);
        }
    }
       
}
