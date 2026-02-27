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
        [HttpPost]
        public async Task<ActionResult<Assignment>> PostAssignment(Assignment assignment)
        {
            ModelState.Remove("Class");

            if (!ModelState.IsValid) return BadRequest(ModelState);

            assignment.Class = null;

            if (assignment.CreatedAt == null) assignment.CreatedAt = DateTime.Now;

            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();
            return Ok(assignment);
        }

        // PUT: api/Assignments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAssignment(int id, [FromBody] Assignment assignment)
        {
            if (id != assignment.AssignmentId) return BadRequest();

            ModelState.Remove("Class");
            assignment.Class = null!; 

            _context.Entry(assignment).State = EntityState.Modified;
           _context.Entry(assignment).Property(x => x.CreatedAt).IsModified = false;

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
        public async Task<IActionResult> GetAssignmentsByLecturer(int lecturerId)
        {
            var data = await _context.Assignments
                .Where(a => a.Class.LecturerId == lecturerId)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new
                {
                    a.AssignmentId,
                    a.ClassId,
                    a.Title,
                    a.Description,
                    a.DueDate,
                    a.CreatedAt,

                    ClassCode = a.Class.ClassCode,
                    SubmissionCount = _context.Submissions
                .Where(s => s.AssignmentId == a.AssignmentId)
                .Select(s => s.StudentId)   // tránh trùng SV nộp nhiều lần
                .Distinct()
                .Count(),
                    
                    TotalStudents = a.Class.Capacity
                })
                .ToListAsync();

            return Ok(data);
        }

        private bool AssignmentExists(int id)
        {
            return _context.Assignments.Any(e => e.AssignmentId == id);
        }
    }
}