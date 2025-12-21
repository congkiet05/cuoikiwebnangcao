using System;
using System.Collections.Generic;

namespace WebQuanLiKhoaHocApi.Entities;

public partial class Registration
{
    public int RegistrationId { get; set; }

    public int StudentId { get; set; }

    public int ClassId { get; set; }

    public DateTime RegisteredAt { get; set; }

    public string Status { get; set; } = null!;

    public virtual Class Class { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
