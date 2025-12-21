using Microsoft.AspNetCore.Mvc;
using WebQuanLiKhoaHocApi.Dtos;
using WebQuanLiKhoaHocApi.Interfaces.HocVien;

namespace WebQuanLiKhoaHocApi.Controllers.HocVien
{
    [Route("api/HocVien/")]
    [ApiController]

    public class HocVien_XemDiemController : Controller
    {
        private readonly IXemDiem _xemDiem;
        public HocVien_XemDiemController(IXemDiem xemDiem)
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
        [HttpGet("XemDiemMon/{MaHocVien}")]
        public async Task<IActionResult> XemDiemMonHoc(string MaHocVien, string MaHocPhan)
        {
            try
            {
                var DiemHocPhan = await _xemDiem.XemDiemMonHoc(MaHocVien , MaHocPhan);
                if (DiemHocPhan == null)
                {
                    return NotFound(new { Message = "Không tìm thấy điểm cho học viên này" });
                }
                return Ok(DiemHocPhan);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi lấy lịch học", Error = ex.Message });
            }
        }
    }
}
