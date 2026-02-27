using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebQuanLiKhoaHocApi.Entities;

namespace WebQuanLiKhoaHocApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly UniversityDBContext _context;

        public NotificationsController(UniversityDBContext context)
        {
            _context = context;
        }

        // 1. Lấy danh sách thông báo của giảng viên (Admin post)
        // GET: api/Notifications/lecturer/2
        [HttpGet("lecturer/{lecturerId}")]
        public async Task<IActionResult> GetNotificationsByLecturer(int lecturerId)
        {
            var notifications = await _context.Notifications
                .Include(n => n.Announcement) // Lấy thông tin từ bảng Announcement
                .Where(n => n.UserId == lecturerId)
                .OrderByDescending(n => n.CreatedAt) // Mới nhất lên đầu
                .Select(n => new
                {
                    n.NotificationId,
                    n.Announcement.Title,
                    n.Announcement.Body,
                    n.IsRead,
                    n.CreatedAt,
                    AuthorName = "Admin" // Vì bạn muốn hiển thị thông báo của Admin
                })
                .ToListAsync();

            return Ok(notifications);
        }

        // 2. Đánh dấu một thông báo là đã đọc
        // PUT: api/Notifications/2/MarkAsRead
        [HttpPut("{id}/MarkAsRead")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null) return NotFound();

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 3. Đánh dấu TẤT CẢ thông báo của giảng viên là đã đọc
        // PUT: api/Notifications/lecturer/2/MarkAllRead
        [HttpPut("lecturer/{lecturerId}/MarkAllRead")]
        public async Task<IActionResult> MarkAllRead(int lecturerId)
        {
            var unreadNotifications = await _context.Notifications
                .Where(n => n.UserId == lecturerId && n.IsRead == false)
                .ToListAsync();

            if (unreadNotifications.Any())
            {
                unreadNotifications.ForEach(n => n.IsRead = true);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}