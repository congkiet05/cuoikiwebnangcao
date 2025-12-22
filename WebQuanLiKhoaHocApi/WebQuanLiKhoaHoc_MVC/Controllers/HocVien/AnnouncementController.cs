using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebQuanLiKhoaHoc_MVC.Service;

namespace WebQuanLiKhoaHoc_MVC.Controllers.HocVien
{
    public class AnnouncementController : Controller
    {
        private readonly HocVien_thongbao _service;

        public AnnouncementController(HocVien_thongbao service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            // Gọi Service đã sửa ở Bước 1
            var data = await _service.GetAnnouncementsAsync();

            // Trả về View
            return View("~/Views/Student/Index.cshtml", data);
        }
    }
}