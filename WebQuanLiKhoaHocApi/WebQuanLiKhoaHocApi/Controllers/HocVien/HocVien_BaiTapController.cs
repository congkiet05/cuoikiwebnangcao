using Microsoft.AspNetCore.Mvc;
using WebQuanLiKhoaHocApi.Dtos;
using WebQuanLiKhoaHocApi.Interfaces.HocVien;

namespace WebQuanLiKhoaHocApi.Controllers.HocVien
{
    [Route("api/BaiTap")]
    [ApiController]
    public class HocVien_BaiTapController : Controller
    {
        private readonly IHocVien_NopBaiTap _service;
        public HocVien_BaiTapController(IHocVien_NopBaiTap service)
        {
            _service = service;
        }
        [HttpGet("ChuaNop")]
        public async Task<IActionResult> LayBaiTapChuaNop(string studentNumber, string classCode)
        {
            if (string.IsNullOrEmpty(studentNumber) || string.IsNullOrEmpty(classCode))
            {
                return BadRequest("Vui lòng cung cấp Mã sinh viên và Mã lớp.");
            }

            try
            {
                var result = await _service.LayBaiTapChuaNop(studentNumber, classCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu cần thiết
                return StatusCode(500, new { Message = "Lỗi Server", Detail = ex.Message });
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetChiTietBaiTap(int id)
        {
            // Gọi Service xử lý, Controller không động vào DbContext
            var result = await _service.LayChiTietBaiTap(id);

            if (result == null)
            {
                return NotFound(new { Message = "Không tìm thấy bài tập này" });
            }

            return Ok(result);
        }
        [HttpPost("NopBai")]
        public async Task<IActionResult> NopBai([FromBody] HocVien_NopBaiDto model)
        {
            if (model == null) return BadRequest("Dữ liệu không hợp lệ");

            // Gọi service xử lý logic lưu vào DB
            var result = await _service.NopBai(model);

            if (result)
            {
                return Ok(new { Message = "Nộp bài thành công!" });
            }

            return BadRequest("Nộp bài thất bại (Không tìm thấy sinh viên hoặc lỗi Server)");
        }
    }
}
