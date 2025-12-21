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
        // 1. Lấy dữ liệu thô từ DB về RAM trước
        var rawMessages = await _context.ClassMessages
            .Where(m => m.ClassId == classId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        // 2. Định dạng hiển thị trên RAM
        var result = rawMessages.Select(m => new {
            m.SenderId,
            m.Content,
            SentAt = m.SentAt.HasValue ? m.SentAt.Value.ToString("HH:mm") : ""
        });

        return Ok(result);
    }
}