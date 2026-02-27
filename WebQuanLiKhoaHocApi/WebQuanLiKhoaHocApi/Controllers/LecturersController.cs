using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebQuanLiKhoaHocApi.Entities;

namespace WebQuanLiKhoaHocApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LecturersController : ControllerBase
    {
        private readonly UniversityDBContext _context;

        public LecturersController(UniversityDBContext context)
        {
            _context = context;
        }

        // GET: api/Lecturers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lecturer>>> GetLecturers()
        {
            return await _context.Lecturers.ToListAsync();
        }

        // GET: api/Lecturers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetLecturer(int id)
        {
            var lecturer = await _context.Lecturers
                .Include(l => l.LecturerNavigation) // Kết nối sang bảng User
                .Where(l => l.LecturerId == id)
                .Select(l => new {
                    // Lấy từ bảng Lecturer
                    LecturerId = l.LecturerId,
                    StaffNumber = l.StaffNumber, // Mã nhân viên
                    FullName = l.FullName,      // Họ tên
                    Department = l.Department,  // Khoa

                    // Lấy từ bảng User (thông qua LecturerNavigation)
                    Username = l.LecturerNavigation.Username, // Tên đăng nhập
                    Email = l.LecturerNavigation.Email        // Email
                })
                .FirstOrDefaultAsync();

            if (lecturer == null) return NotFound();

            return Ok(lecturer);
        }
        [HttpPut("UpdateProfile/{id}")]
        public async Task<IActionResult> PutLecturer(int id, LecturerProfileUpdateDto dto)
        {
            // 1. Lấy dữ liệu hiện tại từ Database kèm theo bảng User (Navigation)
            var lecturer = await _context.Lecturers
                .Include(l => l.LecturerNavigation)
                .FirstOrDefaultAsync(l => l.LecturerId == id);

            if (lecturer == null)
            {
                return NotFound("Không tìm thấy giảng viên");
            }

            // 2. Cập nhật các trường thuộc bảng Lecturer
            lecturer.FullName = dto.FullName;
            lecturer.Department = dto.Department;

            // 3. Cập nhật các trường thuộc bảng User (thông qua Navigation)
            if (lecturer.LecturerNavigation != null)
            {
                lecturer.LecturerNavigation.Email = dto.Email;

                // Chỉ cập nhật mật khẩu nếu giảng viên có nhập mật khẩu mới
                if (!string.IsNullOrEmpty(dto.NewPassword))
                {
                    // Lưu ý: Nếu có cơ chế mã hóa mật khẩu, hãy mã hóa ở đây
                    lecturer.LecturerNavigation.Password = dto.NewPassword;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LecturerExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }        // POST: api/Lecturers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Lecturer>> PostLecturer(Lecturer lecturer)
        {
            _context.Lecturers.Add(lecturer);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LecturerExists(lecturer.LecturerId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLecturer", new { id = lecturer.LecturerId }, lecturer);
        }

        // DELETE: api/Lecturers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLecturer(int id)
        {
            var lecturer = await _context.Lecturers.FindAsync(id);
            if (lecturer == null)
            {
                return NotFound();
            }

            _context.Lecturers.Remove(lecturer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LecturerExists(int id)
        {
            return _context.Lecturers.Any(e => e.LecturerId == id);
        }
    }
}
public class LecturerProfileUpdateDto
{
    public string FullName { get; set; } = null!;
    public string? Department { get; set; }
    public string? Email { get; set; }
    public string? NewPassword { get; set; }
}