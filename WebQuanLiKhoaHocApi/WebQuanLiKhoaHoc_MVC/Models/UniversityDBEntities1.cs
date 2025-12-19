using Microsoft.EntityFrameworkCore;
using WebQuanLiKhoaHocApi.Entities;

namespace WebQuanLiKhoaHocApi.Models
{
    public class UniversityDBEntities1 : DbContext
    {
        public UniversityDBEntities1(DbContextOptions<UniversityDBEntities1> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<ClassSchedule> ClassSchedules { get; set; }
        public DbSet<User> User { get; set; } // Thêm dòng này để định nghĩa tập hợp người dùng
        public DbSet<Role> Role { get; set; }
    }
}