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
    public class AnnouncementsController : ControllerBase
    {
        private readonly UniversityDBContext _context;

        public AnnouncementsController(UniversityDBContext context)
        {
            _context = context;
        }

        // GET: api/Announcements
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Announcement>>> GetAnnouncements()
        {
            return await _context.Announcements.ToListAsync();
        }

        // GET: api/Announcements/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Announcement>> GetAnnouncement(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);

            if (announcement == null)
            {
                return NotFound();
            }

            return announcement;
        }

        // PUT: api/Announcements/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAnnouncement(int id, Announcement dto)
        {
            if (id != dto.AnnouncementId)
                return BadRequest();

            var ann = await _context.Announcements.FindAsync(id);
            if (ann == null)
                return NotFound();

            // CHỈ update field cho phép
            ann.Title = dto.Title;
            ann.Body = dto.Body;
            ann.TargetClassId = dto.TargetClassId;
            ann.IsGlobal = dto.IsGlobal;
            ann.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        // POST: api/Announcements
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Announcement>> PostAnnouncement(Announcement announcement)
        {
            announcement.CreatedAt = DateTime.Now;
            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAnnouncement", new { id = announcement.AnnouncementId }, announcement);
        }

        // DELETE: api/Announcements/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnnouncement(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null)
            {
                return NotFound();
            }

            _context.Announcements.Remove(announcement);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        // GET: api/Announcements/by-lecturer/3
        [HttpGet("by-lecturer/{lecturerId}")]
        public async Task<ActionResult<IEnumerable<Announcement>>> GetAnnouncementsByLecturer(int lecturerId)
        {
            return await _context.Announcements
                .Where(a => a.AuthorId == lecturerId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        private bool AnnouncementExists(int id)
        {
            return _context.Announcements.Any(e => e.AnnouncementId == id);
        }
    }
}
