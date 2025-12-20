using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebQuanLiKhoaHoc_MVC.Models.DTOs;

namespace WebQuanLiKhoaHoc_MVC.Controllers
{
    public class ClassesController : Controller
    {
        private readonly HttpClient _client;

        // DI cho IHttpClientFactory
        public ClassesController(IHttpClientFactory httpClientFactory)
        {
            // Lấy Named Client đã cấu hình
            _client = httpClientFactory.CreateClient("ApiClient");
        }

        // Action cho trang quản lý thông báo
        public async Task<IActionResult> Index()
        {
            // GIẢ ĐỊNH ID: Sử dụng 2 cho mục đích thử nghiệm
            int lecturerId = 2;

            try
            {
                // 1. Gọi API và Deserialize thành List<ClassDto>
                var lecturerClasses = await _client.GetFromJsonAsync<List<ClassDto>>(
                    $"Classes/by-lecturer/{lecturerId}" // Endpoint đã được Include(Course)
                );

                var classes = lecturerClasses ?? new List<ClassDto>();

                // 2. CHUYỂN ĐỔI sang SELECTLISTITEM
                var classSelectList = classes
                    .Select(c => new SelectListItem
                    {
                        Value = c.ClassId.ToString(),
                        // Sử dụng thuộc tính tiện ích (CourseCode, CourseName)
                        Text = $"{c.ClassCode} - {c.CourseName ?? "No Name"} ({c.Semester})"
                    })
                    .ToList();

                // 3. Đưa danh sách đã chuyển đổi vào ViewBag
                ViewBag.TargetClassList = classSelectList;
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage = "Không thể kết nối đến máy chủ API.";
                ViewBag.TargetClassList = new List<SelectListItem>();
            }

            return View();
        }
    }
}
