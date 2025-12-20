using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebQuanLiKhoaHocApi.Entities;

public partial class UniversityDBContext : DbContext
{
    public UniversityDBContext()
    {
    }

    public UniversityDBContext(DbContextOptions<UniversityDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Announcement> Announcements { get; set; }

    public virtual DbSet<Assignment> Assignments { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<ClassMessage> ClassMessages { get; set; }

    public virtual DbSet<ClassSchedule> ClassSchedules { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Lecturer> Lecturers { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Registration> Registrations { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Submission> Submissions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Announcement>(entity =>
        {
            entity.HasKey(e => e.AnnouncementId).HasName("PK__Announce__9DE44574D5E9931E");

            entity.ToTable("Announcement", tb =>
                {
                    tb.HasTrigger("trg_AfterInsertClassAnnouncement");
                    tb.HasTrigger("trg_AfterInsertGlobalAnnouncement");
                });

            entity.HasIndex(e => e.TargetClassId, "IX_Announcement_TargetClass");

            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Title).HasMaxLength(300);
            entity.Property(e => e.UpdatedAt).HasPrecision(3);

            entity.HasOne(d => d.Author).WithMany(p => p.Announcements)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Announcem__Autho__5FB337D6");

            entity.HasOne(d => d.TargetClass).WithMany(p => p.Announcements)
                .HasForeignKey(d => d.TargetClassId)
                .HasConstraintName("FK__Announcem__Targe__619B8048");
        });

        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId).HasName("PK__Assignme__32499E7707A9F406");

            entity.ToTable("Assignment");

            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DueDate).HasPrecision(0);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Class).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__Assignmen__Class__6C190EBB");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK__Class__CB1927C0315B3D6B");

            entity.ToTable("Class");

            entity.HasIndex(e => e.LecturerId, "IX_Class_Lecturer");

            entity.HasIndex(e => e.ClassCode, "UQ__Class__2ECD4A5546F4CEE1").IsUnique();

            entity.Property(e => e.Capacity).HasDefaultValue(50);
            entity.Property(e => e.ClassCode).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Semester).HasMaxLength(20);

            entity.HasOne(d => d.Course).WithMany(p => p.Classes)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__Class__CourseId__4D94879B");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Classes)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Class__CreatedBy__5070F446");

            entity.HasOne(d => d.Lecturer).WithMany(p => p.Classes)
                .HasForeignKey(d => d.LecturerId)
                .HasConstraintName("FK__Class__LecturerI__4F7CD00D");
        });

        modelBuilder.Entity<ClassMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__ClassMes__C87C0C9C6AEB7CC1");

            entity.ToTable("ClassMessage");

            entity.Property(e => e.SentAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Class).WithMany(p => p.ClassMessages)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__ClassMess__Class__7A672E12");

            entity.HasOne(d => d.Sender).WithMany(p => p.ClassMessages)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClassMess__Sende__7B5B524B");
        });

        modelBuilder.Entity<ClassSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__ClassSch__9C8A5B49936A88F7");

            entity.ToTable("ClassSchedule");

            entity.HasIndex(e => e.ClassId, "IX_ClassSchedule_Class");

            entity.HasIndex(e => new { e.ClassId, e.DayOfWeek, e.StartTime }, "UQ_ClassSchedule_Class_Day_Start").IsUnique();

            entity.Property(e => e.EndTime).HasPrecision(0);
            entity.Property(e => e.Room).HasMaxLength(100);
            entity.Property(e => e.StartTime).HasPrecision(0);

            entity.HasOne(d => d.Class).WithMany(p => p.ClassSchedules)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__ClassSche__Class__5535A963");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Course__C92D71A75D05CB57");

            entity.ToTable("Course");

            entity.HasIndex(e => e.CourseCode, "UQ__Course__FC00E000ED709695").IsUnique();

            entity.Property(e => e.CourseCode).HasMaxLength(50);
            entity.Property(e => e.CourseName).HasMaxLength(300);
            entity.Property(e => e.Credit).HasDefaultValue(3);
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__6A4BEDD612C61229");

            entity.ToTable("Feedback");

            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Class).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Feedback__ClassI__74AE54BC");

            entity.HasOne(d => d.Student).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Feedback__Studen__75A278F5");
        });

        modelBuilder.Entity<Lecturer>(entity =>
        {
            entity.HasKey(e => e.LecturerId).HasName("PK__Lecturer__5A78B93D04A28A39");

            entity.ToTable("Lecturer");

            entity.HasIndex(e => e.StaffNumber, "UQ__Lecturer__F2BC669BCC2ACB60").IsUnique();

            entity.Property(e => e.LecturerId).ValueGeneratedNever();
            entity.Property(e => e.Department).HasMaxLength(150);
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.StaffNumber).HasMaxLength(30);

            entity.HasOne(d => d.LecturerNavigation).WithOne(p => p.Lecturer)
                .HasForeignKey<Lecturer>(d => d.LecturerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Lecturer__Lectur__45F365D3");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E1218260319");

            entity.ToTable("Notification");

            entity.HasIndex(e => new { e.UserId, e.AnnouncementId }, "UQ_Notification_User_Announcement").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Announcement).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.AnnouncementId)
                .HasConstraintName("FK__Notificat__Annou__6754599E");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Notificat__UserI__66603565");
        });

        modelBuilder.Entity<Registration>(entity =>
        {
            entity.HasKey(e => e.RegistrationId).HasName("PK__Registra__6EF58810C7657B31");

            entity.ToTable("Registration");

            entity.HasIndex(e => e.ClassId, "IX_Registration_Class");

            entity.HasIndex(e => e.StudentId, "IX_Registration_Student");

            entity.HasIndex(e => new { e.StudentId, e.ClassId }, "UQ__Registra__2E74B9E430340018").IsUnique();

            entity.Property(e => e.RegisteredAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Registered");

            entity.HasOne(d => d.Class).WithMany(p => p.Registrations)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__Registrat__Class__5AEE82B9");

            entity.HasOne(d => d.Student).WithMany(p => p.Registrations)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__Registrat__Stude__59FA5E80");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1AF2235950");

            entity.ToTable("Role");

            entity.HasIndex(e => e.RoleName, "UQ__Role__8A2B6160D672ACDA").IsUnique();

            entity.Property(e => e.RoleId).ValueGeneratedOnAdd();
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Student__32C52B99E22F24D7");

            entity.ToTable("Student");

            entity.HasIndex(e => e.StudentNumber, "UQ__Student__DD81BF6C3E4EA989").IsUnique();

            entity.Property(e => e.StudentId).ValueGeneratedNever();
            entity.Property(e => e.Faculty).HasMaxLength(150);
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.StudentNumber).HasMaxLength(30);

            entity.HasOne(d => d.StudentNavigation).WithOne(p => p.Student)
                .HasForeignKey<Student>(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Student__Student__4222D4EF");
        });

        modelBuilder.Entity<Submission>(entity =>
        {
            entity.HasKey(e => e.SubmissionId).HasName("PK__Submissi__449EE12592BB8A12");

            entity.ToTable("Submission");

            entity.Property(e => e.SubmittedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Assignment).WithMany(p => p.Submissions)
                .HasForeignKey(d => d.AssignmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submissio__Assig__6FE99F9F");

            entity.HasOne(d => d.Student).WithMany(p => p.Submissions)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submissio__Stude__70DDC3D8");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4CA5EC7842");

            entity.ToTable("User");

            entity.HasIndex(e => e.Username, "UQ__User__536C85E44F9E8E3C").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__User__A9D105345BD34D07").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasMaxLength(320);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastLoginAt).HasPrecision(3);
            entity.Property(e => e.Password).HasMaxLength(72);
            entity.Property(e => e.Username).HasMaxLength(100);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User__RoleId__3C69FB99");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
