using System;
using System.Collections.Generic;

namespace WebQuanLiKhoaHocApi.Entities;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public int ClassId { get; set; }

    public int StudentId { get; set; }

    public byte? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
