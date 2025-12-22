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
    public class ClassesController : ControllerBase
    {
        private readonly UniversityDBContext _context;

        public ClassesController(UniversityDBContext context)
        {
            _context = context;
        }

        // GET: api/Classes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Class>>> GetClasses()
        {
            return await _context.Classes.ToListAsync();
        }

        // GET: api/Classes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Class>> GetClass(int id)
        {
            var @class = await _context.Classes.FindAsync(id);

            if (@class == null)
            {
                return NotFound();
            }

            return @class;
        }

        // PUT: api/Classes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClass(int id, Class @class)
        {
            if (id != @class.ClassId)
            {
                return BadRequest();
            }

            _context.Entry(@class).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClassExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Classes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Class>> PostClass(Class @class)
        {
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClass", new { id = @class.ClassId }, @class);
        }

        // DELETE: api/Classes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClass(int id)
        {
            var @class = await _context.Classes.FindAsync(id);
            if (@class == null)
            {
                return NotFound();
            }

            _context.Classes.Remove(@class);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Classes/Lecturer/2
        [HttpGet("Lecturer/{lecturerId}")]
        public async Task<IActionResult> GetClassesByLecturer(int lecturerId)
        {
            var classes = await _context.Classes
                .Include(c => c.Course) // Load thông tin môn học để lấy tên môn
                .Where(c => c.LecturerId == lecturerId)
                .Select(c => new {
                    c.ClassId,
                    c.ClassCode,
                    CourseName = c.Course != null ? c.Course.CourseName : "No Course Name"
                })
                .ToListAsync();

            return Ok(classes);
        }

        // GET: api/Classes/Student/5
        [HttpGet("Student/{studentId}")]
        public async Task<IActionResult> GetClassesByStudent(int studentId)
        {
            var classes = await _context.Registrations
                .Where(r => r.StudentId == studentId && r.Status == "Registered") // Chỉ lấy lớp đã đăng ký học
                .Include(r => r.Class)
                    .ThenInclude(c => c.Course)
                .Select(r => new {
                    r.Class.ClassId,
                    r.Class.ClassCode,
                    CourseName = r.Class.Course != null ? r.Class.Course.CourseName : "No Course Name"
                })
                .ToListAsync();

            return Ok(classes);
        }

        private bool ClassExists(int id)
        {
            return _context.Classes.Any(e => e.ClassId == id);
        }
    }
}
