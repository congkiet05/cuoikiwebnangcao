using System;
using System.Collections.Generic;

namespace WebQuanLiKhoaHocApi.Entities;

public partial class Class
{
    public int ClassId { get; set; }

    public int CourseId { get; set; }

    public string ClassCode { get; set; } = null!;

    public string Semester { get; set; } = null!;

    public int Capacity { get; set; }

    public int? LecturerId { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual ICollection<ClassMessage> ClassMessages { get; set; } = new List<ClassMessage>();

    public virtual ICollection<ClassSchedule> ClassSchedules { get; set; } = new List<ClassSchedule>();

    public virtual Course Course { get; set; } = null!;

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual Lecturer? Lecturer { get; set; }

    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}
