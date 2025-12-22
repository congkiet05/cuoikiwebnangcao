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
            // 1. Khởi tạo đối tượng với đầy đủ thông tin (Thay cho dấu ... bị lỗi)
            var newMessage = new ClassMessage
            {
                ClassId = classId,
                SenderId = senderId,
                Content = content,
                SentAt = DateTime.Now // Gán thời gian hiện tại
            };

            // 2. Lưu vào Database
            _context.ClassMessages.Add(newMessage);
            await _context.SaveChangesAsync();

            // 3. Lấy Tên hiển thị (FullName) từ bảng Student hoặc Lecturer
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == senderId);
            var lecturer = await _context.Lecturers.FirstOrDefaultAsync(l => l.LecturerId == senderId);

            // Ưu tiên lấy FullName, nếu không có thì lấy Username từ bảng User
            string senderName = student?.FullName ?? lecturer?.FullName ??
                               (await _context.Users.FindAsync(senderId))?.Username ?? "Unknown";

            // 4. Gửi tin nhắn đến Group lớp học
            // Sử dụng định dạng giờ HH:mm (Ví dụ: 14:30)
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