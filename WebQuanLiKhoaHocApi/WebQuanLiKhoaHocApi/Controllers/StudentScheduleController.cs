using Microsoft.AspNetCore.Mvc;
using WebQuanLiKhoaHocApi.Services;

namespace WebQuanLiKhoaHocApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentScheduleController : Controller
    {
        private readonly IScheduleService _scheduleService;
        public StudentScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

       
    }
}
