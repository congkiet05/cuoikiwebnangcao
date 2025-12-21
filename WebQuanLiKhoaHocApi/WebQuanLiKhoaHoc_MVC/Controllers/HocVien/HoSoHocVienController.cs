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
            if(string.IsNullOrEmpty(MaHocVien))
            {
                return BadRequest("Mã học viên không được để trống");
            }

            var hocVien = await hoSoHocVienService.LayHoSoHocVien(MaHocVien);

            if(hocVien == null)
            {
                ViewBag.ErrorMessage = "Không tìm thấy học viên này";
                return View("Error");
            }
            return View("~/Views/Student/HoSoHocVien.cshtml", hocVien);
        }
    }
       
}
