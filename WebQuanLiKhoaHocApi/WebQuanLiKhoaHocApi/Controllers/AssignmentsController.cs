using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebQuanLiKhoaHocApi.Entities;

namespace WebQuanLiKhoaHocApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentsController : ControllerBase
    {
        private readonly UniversityDBContext _context;

        public AssignmentsController(UniversityDBContext context)
        {
            _context = context;
        }

        // GET: api/Assignments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Assignment>>> GetAssignments()
        {
            return await _context.Assignments
                .Include(a => a.Class) // Lấy kèm thông tin lớp học
                .ToListAsync();
        }

        // GET: api/Assignments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Assignment>> GetAssignment(int id)
        {
            var assignment = await _context.Assignments
                .Include(a => a.Class)
                .FirstOrDefaultAsync(a => a.AssignmentId == id);

            if (assignment == null) return NotFound();

            return assignment;
        }

        // POST: api/Assignments
        [HttpPost]
        public async Task<ActionResult<Assignment>> PostAssignment(Assignment assignment)
        {
            // Loại bỏ kiểm tra navigation property để tránh lỗi 400 Bad Request
            ModelState.Remove("Class");

            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Gán ngày tạo mặc định nếu phía MVC không gửi
            if (assignment.CreatedAt == null)
                assignment.CreatedAt = DateTime.Now;

            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAssignment", new { id = assignment.AssignmentId }, assignment);
        }

        // PUT: api/Assignments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAssignment(int id, Assignment assignment)
        {
            if (id != assignment.AssignmentId) return BadRequest();

            ModelState.Remove("Class");
            _context.Entry(assignment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssignmentExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // DELETE: api/Assignments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null) return NotFound();

            _context.Assignments.Remove(assignment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("by-lecturer/{lecturerId}")]
        public async Task<ActionResult<IEnumerable<Assignment>>> GetAssignmentsByLecturer(int lecturerId)
        {
            // 1. Lấy danh sách bài tập của giảng viên này
            var assignments = await _context.Assignments
                .Include(a => a.Class)
                .Where(a => a.Class.LecturerId == lecturerId)
                .ToListAsync();

            // 2. Chạy vòng lặp để đếm số bài nộp cho từng bài tập
            foreach (var a in assignments)
            {
                // Đếm xem trong bảng Submission có bao nhiêu dòng ứng với AssignmentId này
                a.SubmissionCount = await _context.Submissions
                    .CountAsync(s => s.AssignmentId == a.AssignmentId);

                // Lấy sĩ số lớp từ bảng Class
                a.TotalStudents = a.Class?.Capacity ?? 0;

                // Lấy mã lớp
                a.ClassCode = a.Class?.ClassCode;
            }

            return Ok(assignments);
        }
        private bool AssignmentExists(int id)
        {
            return _context.Assignments.Any(e => e.AssignmentId == id);
        }
    }
}