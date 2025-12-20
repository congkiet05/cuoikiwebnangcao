using System;
using System.Collections.Generic;

namespace WebQuanLiKhoaHocApi.Entities;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public int AnnouncementId { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Announcement Announcement { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
