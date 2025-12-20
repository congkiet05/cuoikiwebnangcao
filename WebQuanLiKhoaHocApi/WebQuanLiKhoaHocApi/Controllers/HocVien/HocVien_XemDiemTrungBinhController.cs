using Microsoft.AspNetCore.Mvc;
using WebQuanLiKhoaHocApi.Interfaces.HocVien;

namespace WebQuanLiKhoaHocApi.Controllers.HocVien
{
    [Route("api/HocVien/")]
    [ApiController]

    public class HocVien_XemDiemTrungBinhController : Controller
    {
        private readonly IXemDiem _xemDiem;
        public HocVien_XemDiemTrungBinhController(IXemDiem xemDiem)
        {
            this._xemDiem = xemDiem;
        }
        [HttpGet("XemDiemTrungBinh/{MaHocVien}")]
        public async Task<IActionResult> XemDiemTrungBinhTatCaMon(string MaHocVien)
        {
            try
            {
                var diemTrungBinh = await _xemDiem.XemDiemTrungBinhTatCaCacMon(MaHocVien);
                if (diemTrungBinh == null)
                {
                    return NotFound(new { Message = "Không tìm thấy điểm cho học viên này" });
                }
                return Ok(diemTrungBinh);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi lấy lịch học", Error = ex.Message });
            }
        }

    }
}
