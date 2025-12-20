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
        public async Task<ActionResult<Lecturer>> GetLecturer(int id)
        {
            // Sử dụng Include để lấy thông tin từ bảng User (LecturerNavigation)
            var lecturer = await _context.Lecturers
                .Include(l => l.LecturerNavigation)
                .FirstOrDefaultAsync(l => l.LecturerId == id);

            if (lecturer == null)
            {
                return NotFound();
            }

            // Gán dữ liệu từ bảng User vào các trường [NotMapped] của Lecturer
            lecturer.Email = lecturer.LecturerNavigation.Email;
            lecturer.Username = lecturer.LecturerNavigation.Username;

            return lecturer;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutLecturer(int id, Lecturer lecturer)
        {
            var existingLecturer = await _context.Lecturers
                .Include(l => l.LecturerNavigation)
                .FirstOrDefaultAsync(l => l.LecturerId == id);

            if (existingLecturer == null) return NotFound();

            try
            {
                // 1. Cập nhật thông tin giảng viên
                existingLecturer.FullName = lecturer.FullName;
                existingLecturer.Department = lecturer.Department;

                if (existingLecturer.LecturerNavigation != null)
                {
                    // 2. Cập nhật Email
                    existingLecturer.LecturerNavigation.Email = lecturer.Email;

                    // 3. CẬP NHẬT MẬT KHẨU (Nếu có nhập mới)
                    if (!string.IsNullOrEmpty(lecturer.NewPassword))
                    {
                        // Lưu ý: Nên mã hóa mật khẩu trước khi lưu nếu hệ thống có dùng Bcrypt/Identity
                        existingLecturer.LecturerNavigation.Password = lecturer.NewPassword;
                    }
                }

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi: " + ex.Message);
            }
        }
        // POST: api/Lecturers
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
        [HttpGet("by-lecturer/{lecturerId}")]
        public async Task<ActionResult<IEnumerable<Class>>> GetClassesByLecturer(int lecturerId)
        {
            var classes = await _context.Classes
                .Include(c => c.Course) // rất quan trọng
                .Where(c => c.LecturerId == lecturerId)
                .ToListAsync();

            if (!classes.Any())
                return NotFound("Lecturer has no classes.");

            return classes;
        }
    }


}
