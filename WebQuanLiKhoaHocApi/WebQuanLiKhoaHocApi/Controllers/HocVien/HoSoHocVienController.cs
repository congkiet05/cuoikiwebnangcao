using Microsoft.AspNetCore.Mvc;
using WebQuanLiKhoaHocApi.Dtos;

using Microsoft.EntityFrameworkCore;
using WebQuanLiKhoaHocApi.Entities;
using WebQuanLiKhoaHocApi.Interfaces.HocVien;
using NuGet.Protocol.Plugins;
using WebQuanLiKhoaHocApi.Services.HocVien;

namespace WebQuanLiKhoaHocApi.Controllers.HocVien
{
    [Route("api/HocVien")]
    [ApiController]
    public class HoSoHocVienController : ControllerBase
    {
        private readonly IHoSoHocVien _hoSoHocVienService;
        public HoSoHocVienController(IHoSoHocVien hoSoHocVienService)
        {
            _hoSoHocVienService = hoSoHocVienService;
        }
        [HttpGet("HoSoHocVien/{MaHocVien}")]
        public async Task<ActionResult<HocVien_HoSoCaNhan?>> LayHoSoHocVien(string MaHocVien)
        {
            var hoSoHocVien = await _hoSoHocVienService.LayHoSoHocvien(MaHocVien);
            if (hoSoHocVien == null)
            {
                return NotFound(new {Message = "Không tìm thấy học viên này"});
            }
            return Ok(hoSoHocVien);
        }
    }
}
