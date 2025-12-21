using Microsoft.AspNetCore.Mvc;
using WebQuanLiKhoaHocApi.Interfaces.HocVien;

namespace WebQuanLiKhoaHocApi.Controllers.HocVien
{
    [Route("api/HocVien/[controller]")]
    [ApiController]
    public class LichHocController : Controller
    {
        private readonly ILichHoc _lichHocService;
        public LichHocController(ILichHoc lichHocService)
        {
            _lichHocService = lichHocService;
        }
        [HttpGet("{MaHocVien}")]
        public async Task<IActionResult> LayLichHoc(string MaHocVien)
        {
            try
            {
                var lichHoc = await _lichHocService.XemLichHoc(MaHocVien);
                if (lichHoc == null)
                {
                    return NotFound(new { Message = "Không tìm thấy lịch học cho học viên này" });
                }
                return Ok(lichHoc);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi lấy lịch học", Error = ex.Message });
            }
        }
    } 
}
