using System.ComponentModel.DataAnnotations.Schema;

namespace WebQuanLiKhoaHocApi.Entities;

public partial class Lecturer
{
    public int LecturerId { get; set; }

    // Sửa thành ? để không bắt buộc khi nhận dữ liệu từ MVC gửi sang
    public string? StaffNumber { get; set; }

    public string FullName { get; set; } = null!;

    public string? Department { get; set; }

    [NotMapped]
    public string? Email { get; set; }

    [NotMapped]
    public string? Username { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    // RẤT QUAN TRỌNG: Sửa thành ? để API không yêu cầu object User đi kèm
    public virtual User? LecturerNavigation { get; set; }
    [NotMapped]
    public string? NewPassword { get; set; }
}