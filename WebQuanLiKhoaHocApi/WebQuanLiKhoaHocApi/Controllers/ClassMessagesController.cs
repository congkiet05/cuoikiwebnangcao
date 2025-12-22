using Microsoft.AspNetCore.Mvc;
using WebQuanLiKhoaHocApi.Entities;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class ClassMessagesController : ControllerBase
{
    private readonly UniversityDBContext _context;

    public ClassMessagesController(UniversityDBContext context)
    {
        _context = context;
    }

    // GET: api/ClassMessages/5
    [HttpGet("{classId}")]
    public async Task<IActionResult> GetMessages(int classId)
    {
        var messages = await _context.ClassMessages
            .Include(m => m.Sender) // Join bảng User
            .Where(m => m.ClassId == classId)
            .OrderBy(m => m.SentAt)
            .Select(m => new {
                m.SenderId,
                // Lấy FullName từ Student hoặc Lecturer tương ứng
                SenderName = _context.Students.Where(s => s.StudentId == m.SenderId).Select(s => s.FullName).FirstOrDefault()
                             ?? _context.Lecturers.Where(l => l.LecturerId == m.SenderId).Select(l => l.FullName).FirstOrDefault()
                             ?? "Hệ thống",
                m.Content,
                SentAt = m.SentAt.HasValue ? m.SentAt.Value.ToString("HH:mm") : ""
            })
            .ToListAsync();

        return Ok(messages);
    }
}