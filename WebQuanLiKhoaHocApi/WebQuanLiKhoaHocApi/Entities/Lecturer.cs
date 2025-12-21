using System;
using System.Collections.Generic;

namespace WebQuanLiKhoaHocApi.Entities;

public partial class Lecturer
{
    public int LecturerId { get; set; }

    public string StaffNumber { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Department { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual User LecturerNavigation { get; set; } = null!;
}
