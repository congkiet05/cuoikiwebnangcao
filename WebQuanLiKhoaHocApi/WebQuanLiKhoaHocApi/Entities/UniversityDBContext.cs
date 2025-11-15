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

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<ClassSchedule> ClassSchedules { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Lecturer> Lecturers { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Registration> Registrations { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Announcement>(entity =>
        {
            entity.HasKey(e => e.AnnouncementId).HasName("PK__Announce__9DE4457426116867");

            entity.ToTable("Announcement");

            entity.HasIndex(e => e.TargetClassId, "IX_Announcement_TargetClass");

            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Title).HasMaxLength(300);
            entity.Property(e => e.UpdatedAt).HasPrecision(3);

            entity.HasOne(d => d.Author).WithMany(p => p.Announcements)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Announcem__Autho__60A75C0F");

            entity.HasOne(d => d.TargetClass).WithMany(p => p.Announcements)
                .HasForeignKey(d => d.TargetClassId)
                .HasConstraintName("FK__Announcem__Targe__628FA481");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK__Class__CB1927C05CF6FDD3");

            entity.ToTable("Class");

            entity.HasIndex(e => e.LecturerId, "IX_Class_Lecturer");

            entity.HasIndex(e => e.ClassCode, "UQ__Class__2ECD4A55D2C896F3").IsUnique();

            entity.Property(e => e.Capacity).HasDefaultValue(50);
            entity.Property(e => e.ClassCode).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Semester).HasMaxLength(20);

            entity.HasOne(d => d.Course).WithMany(p => p.Classes)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__Class__CourseId__4E88ABD4");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Classes)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Class__CreatedBy__5165187F");

            entity.HasOne(d => d.Lecturer).WithMany(p => p.Classes)
                .HasForeignKey(d => d.LecturerId)
                .HasConstraintName("FK__Class__LecturerI__5070F446");
        });

        modelBuilder.Entity<ClassSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__ClassSch__9C8A5B491AB1CCFE");

            entity.ToTable("ClassSchedule");

            entity.HasIndex(e => e.ClassId, "IX_ClassSchedule_Class");

            entity.HasIndex(e => new { e.ClassId, e.DayOfWeek, e.StartTime }, "UQ_ClassSchedule_Class_Day_Start").IsUnique();

            entity.Property(e => e.EndTime).HasPrecision(0);
            entity.Property(e => e.Room).HasMaxLength(100);
            entity.Property(e => e.StartTime).HasPrecision(0);

            entity.HasOne(d => d.Class).WithMany(p => p.ClassSchedules)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__ClassSche__Class__5629CD9C");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Course__C92D71A769A292BE");

            entity.ToTable("Course");

            entity.HasIndex(e => e.CourseCode, "UQ__Course__FC00E000A90547C7").IsUnique();

            entity.Property(e => e.CourseCode).HasMaxLength(50);
            entity.Property(e => e.CourseName).HasMaxLength(300);
            entity.Property(e => e.Credit).HasDefaultValue(3);
        });

        modelBuilder.Entity<Lecturer>(entity =>
        {
            entity.HasKey(e => e.LecturerId).HasName("PK__Lecturer__5A78B93D9CB2BAD9");

            entity.ToTable("Lecturer");

            entity.HasIndex(e => e.StaffNumber, "UQ__Lecturer__F2BC669BD262233E").IsUnique();

            entity.Property(e => e.LecturerId).ValueGeneratedNever();
            entity.Property(e => e.Department).HasMaxLength(150);
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.StaffNumber).HasMaxLength(30);

            entity.HasOne(d => d.LecturerNavigation).WithOne(p => p.Lecturer)
                .HasForeignKey<Lecturer>(d => d.LecturerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Lecturer__Lectur__46E78A0C");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E1294D67D72");

            entity.ToTable("Notification");

            entity.HasIndex(e => new { e.UserId, e.AnnouncementId }, "UQ_Notification_User_Announcement").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Announcement).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.AnnouncementId)
                .HasConstraintName("FK__Notificat__Annou__68487DD7");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Notificat__UserI__6754599E");
        });

        modelBuilder.Entity<Registration>(entity =>
        {
            entity.HasKey(e => e.RegistrationId).HasName("PK__Registra__6EF58810A0F151B0");

            entity.ToTable("Registration");

            entity.HasIndex(e => e.ClassId, "IX_Registration_Class");

            entity.HasIndex(e => e.StudentId, "IX_Registration_Student");

            entity.HasIndex(e => new { e.StudentId, e.ClassId }, "UQ__Registra__2E74B9E477101375").IsUnique();

            entity.Property(e => e.RegisteredAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Registered");

            entity.HasOne(d => d.Class).WithMany(p => p.Registrations)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__Registrat__Class__5BE2A6F2");

            entity.HasOne(d => d.Student).WithMany(p => p.Registrations)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__Registrat__Stude__5AEE82B9");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1AE61B5194");

            entity.ToTable("Role");

            entity.HasIndex(e => e.RoleName, "UQ__Role__8A2B6160D8C84C44").IsUnique();

            entity.Property(e => e.RoleId).ValueGeneratedOnAdd();
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Student__32C52B99D55D55A3");

            entity.ToTable("Student");

            entity.HasIndex(e => e.StudentNumber, "UQ__Student__DD81BF6CD9D98B6F").IsUnique();

            entity.Property(e => e.StudentId).ValueGeneratedNever();
            entity.Property(e => e.Faculty).HasMaxLength(150);
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.StudentNumber).HasMaxLength(30);

            entity.HasOne(d => d.StudentNavigation).WithOne(p => p.Student)
                .HasForeignKey<Student>(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Student__Student__4316F928");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4C87B2083B");

            entity.ToTable("User");

            entity.HasIndex(e => e.Username, "UQ__User__536C85E477CF9880").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__User__A9D1053471C575E2").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(320);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastLoginAt).HasPrecision(3);
            entity.Property(e => e.PasswordHash).HasMaxLength(72);
            entity.Property(e => e.Username).HasMaxLength(100);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User__RoleId__3D5E1FD2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
