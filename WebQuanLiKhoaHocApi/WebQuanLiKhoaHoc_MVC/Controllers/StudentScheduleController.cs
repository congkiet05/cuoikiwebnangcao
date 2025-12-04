using Microsoft.AspNetCore.Mvc;
using WebQuanLiKhoaHoc_MVC.Interface;

namespace WebQuanLiKhoaHoc_MVC.Controllers
{
    public class StudentScheduleController : Controller
    {
        private readonly IScheduleApiService _apiService;
        public StudentScheduleController(IScheduleApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var schedule = await _apiService.GetStudentScheduleAsync();
            var sortedSchedule = schedule
                .OrderBy(s => s.NgayTrongTuan)
                .ThenBy(s => s.ThoiGianBatDau)
                .ToList();

            // Truyền View Model (danh sách lịch học) sang View
            return View(sortedSchedule);
        }
    }
}
