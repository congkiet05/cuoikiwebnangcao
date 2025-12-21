using System;
using System.Collections.Generic;

namespace WebQuanLiKhoaHocApi.Entities;

public partial class Announcement
{
    public int AnnouncementId { get; set; }

    public string Title { get; set; } = null!;

    public string Body { get; set; } = null!;

    public int AuthorId { get; set; }

    public bool IsGlobal { get; set; }

    public int? TargetClassId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? Author { get; set; } = null!;

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Class? TargetClass { get; set; }
}
