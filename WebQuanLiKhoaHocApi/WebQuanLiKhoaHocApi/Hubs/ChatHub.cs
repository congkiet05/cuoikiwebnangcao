using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebQuanLiKhoaHocApi.Entities;

namespace WebQuanLiKhoaHocApi.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly UniversityDBContext _context;

        public ChatHub(UniversityDBContext context)
        {
            _context = context;
        }

        public async Task JoinClassGroup(string classId)
        {
            // Khi người dùng click vào lớp nào, họ sẽ join vào group lớp đó
            await Groups.AddToGroupAsync(Context.ConnectionId, classId);
        }

        public async Task SendMessage(int classId, string content)
        {
            // Lấy ID người dùng từ Token (đã khớp với cấu hình trong AuthController)
            var userIdStr = Context.User.FindFirst("userId")?.Value;

            if (!int.TryParse(userIdStr, out int senderId))
            {
                throw new HubException("Invalid senderId from token");
            }

            // 1. Logic lưu tin nhắn vào Database thật
            var newMessage = new ClassMessage // Giả sử tên thực thể của bạn là ClassMessage
            {
                ClassId = classId,
                SenderId = senderId,
                Content = content,
                SentAt = DateTime.Now
            };

            _context.ClassMessages.Add(newMessage);
            await _context.SaveChangesAsync();

            // 2. Gửi tin nhắn real-time cho cả lớp (bao gồm cả người gửi)
            await Clients.Group(classId.ToString()).SendAsync("ReceiveMessage", new
            {
                ClassId = classId,
                SenderId = senderId,
                Content = content,
                SentAt = newMessage.SentAt.ToString()
            });
        }
    }
}