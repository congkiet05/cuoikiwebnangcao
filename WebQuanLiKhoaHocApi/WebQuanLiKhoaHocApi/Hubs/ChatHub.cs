using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.EntityFrameworkCore;
using WebQuanLiKhoaHocApi.Entities;

namespace WebQuanLiKhoaHocApi.Hubs
{
    public class ChatHub : Hub
    {
        private readonly UniversityDBContext _context;

        public ChatHub(UniversityDBContext context)
        {
            _context = context;
        }

        // Khi giảng viên chọn một lớp, họ sẽ "Tham gia" vào một Group riêng của lớp đó
        public async Task JoinClassGroup(string classId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, classId);
        }

        // Phương thức gửi tin nhắn
        public async Task SendMessage(int classId, int senderId, string content)
        {
            // Bước 1: Lưu vào cơ sở dữ liệu
            var newMessage = new ClassMessage
            {
                ClassId = classId,
                SenderId = senderId,
                Content = content,
                SentAt = DateTime.Now
            };

            _context.ClassMessages.Add(newMessage);
            await _context.SaveChangesAsync();

            // Bước 2: Lấy thông tin người gửi (để hiển thị tên trên giao diện chat)
            var sender = await _context.Users.FindAsync(senderId);
            var senderName = sender?.Username ?? "Unknown";

            // Bước 3: Đẩy tin nhắn tới TẤT CẢ mọi người trong Group (lớp học) này
            // Client sẽ lắng nghe sự kiện tên là "ReceiveMessage"
            await Clients.Group(classId.ToString()).SendAsync("ReceiveMessage", new
            {
                SenderId = senderId,
                SenderName = senderName,
                Content = content,
                SentAt = newMessage.SentAt.Value.ToString("HH:mm")
            });
        }
    }
}