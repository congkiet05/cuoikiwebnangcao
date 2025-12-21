using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebQuanLiKhoaHocApi.DTOs;
using WebQuanLiKhoaHocApi.Entities;
using WebQuanLiKhoaHocApi; // Đảm bảo using đúng namespace chứa Entity và DbContext

namespace WebQuanLiKhoaHocApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly UniversityDBContext _context; // Thay tên DbContext của bạn vào đây

        public NotificationController(UniversityDBContext context)
        {
            _context = context;
        }

        // GET: api/Notification/get-student-notifs/{studentId}
        [HttpGet("get-student-notifs/{studentId}")]
        public async Task<IActionResult> GetStudentNotifications(int studentId)
        {
            // 1. Lấy danh sách ClassId mà sinh viên đang học (Status = Registered)
            var registeredClassIds = await _context.Registrations
                .Where(r => r.StudentId == studentId && r.Status == "Registered")
                .Select(r => r.ClassId)
                .ToListAsync();

            // 2. Query lấy thông báo:
            // - Điều kiện: Là thông báo Global HOẶC thông báo của lớp sinh viên đang học
            // - Left Join bảng Notification để biết sinh viên đã đọc chưa
            var query = from a in _context.Announcements
                        join n in _context.Notifications
                             on new { AId = a.AnnouncementId, UId = studentId }
                             equals new { AId = n.AnnouncementId, UId = n.UserId } into userNotif
                        from un in userNotif.DefaultIfEmpty()

                        where a.IsGlobal == true || (a.TargetClassId.HasValue && registeredClassIds.Contains(a.TargetClassId.Value))

                        orderby a.CreatedAt descending
                        select new NotificationDTO
                        {
                            AnnouncementId = a.AnnouncementId,
                            Title = a.Title,
                            Body = a.Body,
                            AuthorName = _context.Users.Where(u => u.UserId == a.AuthorId).Select(u => u.Username).FirstOrDefault(),
                            CreatedAt = a.CreatedAt,
                            IsRead = (un != null && un.IsRead), // True nếu tìm thấy record
                            Type = a.IsGlobal ? "Toàn trường" : "Lớp học phần"
                        };

            var result = await query.ToListAsync();
            return Ok(result);
        }
        // GET: api/Notification/get-all-list/{studentId}
        [HttpGet("get-all-list/{studentId}")]
        public async Task<IActionResult> GetAllAnnouncementsForPage(int studentId)
        {
            // 1. Lấy danh sách lớp
            var registeredClassIds = await _context.Registrations
                .Where(r => r.StudentId == studentId && r.Status == "Registered")
                .Select(r => r.ClassId)
                .ToListAsync();

            // 2. Lấy thông báo (Global + Lớp)
            var query = from a in _context.Announcements
                            // Join Author để lấy tên giảng viên/admin
                        join u in _context.Users on a.AuthorId equals u.UserId

                        // Left join Notification để biết đã đọc chưa (nếu muốn hiển thị trạng thái)
                        join n in _context.Notifications
                             on new { AId = a.AnnouncementId, UId = studentId }
                             equals new { AId = n.AnnouncementId, UId = n.UserId } into userNotif
                        from un in userNotif.DefaultIfEmpty()

                        where a.IsGlobal == true || (a.TargetClassId.HasValue && registeredClassIds.Contains(a.TargetClassId.Value))

                        orderby a.CreatedAt descending
                        select new NotificationDTO
                        {
                            AnnouncementId = a.AnnouncementId,
                            Title = a.Title,
                            Body = a.Body,
                            AuthorName = u.Username, // Hoặc u.FullName
                            CreatedAt = a.CreatedAt,
                            IsRead = (un != null && un.IsRead),
                            Type = a.IsGlobal ? "Toàn trường" : "Lớp học phần"
                        };

            var data = await query.ToListAsync();
            return Ok(data);
        }

        // POST: api/Notification/mark-read
        [HttpPost("mark-read")]
        public async Task<IActionResult> MarkAsRead(int studentId, int announcementId)
        {
            // Kiểm tra xem đã có record trong bảng Notification chưa
            var notif = await _context.Notifications
                .FirstOrDefaultAsync(n => n.UserId == studentId && n.AnnouncementId == announcementId);

            if (notif == null)
            {
                // Chưa có => Tạo mới (đánh dấu là đã đọc)
                notif = new Notification
                {
                    UserId = studentId,
                    AnnouncementId = announcementId,
                    IsRead = true,
                    CreatedAt = DateTime.Now
                };
                _context.Notifications.Add(notif);
            }
            else
            {
                // Có rồi => Update lại cho chắc chắn
                notif.IsRead = true;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Success" });
        }
    }
}