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

        [HttpGet("MySchedule")]
        public async Task<IActionResult> getMySchedule()
        {
            var studentId = 1;
            var schedule = await _scheduleService.GetStudentScheduleAsync(studentId);
            if(schedule == null || !schedule.Any())
            {
                return NotFound("No schedule found for the student.");
            }
            return Ok(schedule);
        }
    }
}
