using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebQuanLiKhoaHocApi.Entities;
using WebQuanLiKhoaHocApi.Dtos;

namespace WebQuanLiKhoaHocApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Admin")]
    public class AdminApiController : ControllerBase
    {
        private readonly UniversityDBContext _context;

        public AdminApiController(UniversityDBContext context)
        {
            _context = context;
        }

        // 1. CREATE USER
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] UserDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.PasswordHash))
                return BadRequest("Vui lòng nhập Username và Password.");

            if (await _context.Users.AnyAsync(x => x.Username == model.Username))
                return BadRequest($"Tài khoản '{model.Username}' đã tồn tại.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // A. Lưu bảng User
                var user = new User
                {
                    Username = model.Username,
                    PasswordHash = model.PasswordHash, // (hash sau)
                    Email = model.Email,
                    IsActive = model.TrangThai == 2,
                    RoleId = (byte)model.RoleId
                    // ❌ KHÔNG GÁN CreatedAt
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // B. Lưu bảng con
                if (model.RoleId == 3) // GIẢNG VIÊN
                {
                    _context.Lecturers.Add(new Lecturer
                    {
                        LecturerId = user.UserId,
                        FullName = model.HoTen ?? "Giảng viên mới",
                        Department = model.BoMon,
                        StaffNumber = "GV" + user.UserId.ToString("D4")
                    });
                }
                else if (model.RoleId == 2) // SINH VIÊN
                {
                    _context.Students.Add(new Student
                    {
                        StudentId = user.UserId,
                        FullName = model.HoTen ?? "Sinh viên mới",
                        Faculty = model.BoMon,
                        StudentNumber = "SV" + user.UserId.ToString("D4"),
                        Year = DateTime.Now.Year
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Thêm thành công" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await (
                from u in _context.Users
                join s in _context.Students on u.UserId equals s.StudentId into us
                from subStudent in us.DefaultIfEmpty()
                join l in _context.Lecturers on u.UserId equals l.LecturerId into ul
                from subLecturer in ul.DefaultIfEmpty()
                orderby u.UserId descending
                select new UserDto
                {
                    Id = u.UserId,
                    Username = u.Username,
                    PasswordHash = u.PasswordHash,
                    Email = u.Email,
                    TrangThai = u.IsActive ? 2 : 0,
                    RoleId = u.RoleId,
                    RoleName = u.RoleId == 4 ? "Admin" : (u.RoleId == 3 ? "Giảng Viên" : "Sinh Viên"),
                    HoTen = subStudent != null
                                ? subStudent.FullName
                                : (subLecturer != null
                                    ? subLecturer.FullName
                                    : "Admin"),

                    BoMon = subStudent != null
                                ? subStudent.Faculty
                                : (subLecturer != null
                                    ? subLecturer.Department
                                    : "Quản trị viên"),

                    CreatedAt = u.CreatedAt,
                    LastLoginAt = u.LastLoginAt
                }
            ).ToListAsync();

            return Ok(users);
        }

        // ...
        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto model)
        {
            // Code Update cũ vẫn dùng tốt, chỉ cần đảm bảo mapping đúng
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            user.Email = model.Email;
            user.IsActive = model.TrangThai == 2;
            if (!string.IsNullOrWhiteSpace(model.PasswordHash)) user.PasswordHash = model.PasswordHash;

            if (user.RoleId == 3) // Update GV
            {
                var lecturer = await _context.Lecturers.FindAsync(id);
                if (lecturer != null)
                {
                    lecturer.FullName = model.HoTen;
                    lecturer.Department = model.BoMon; // Cập nhật Bộ môn
                }
            }
            else if (user.RoleId == 2) // Update SV
            {
                var student = await _context.Students.FindAsync(id);
                if (student != null)
                {
                    student.FullName = model.HoTen;
                    student.Faculty = model.BoMon; // Cập nhật Môn học
                }
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            var student = await _context.Students.FindAsync(id);
            if (student != null) _context.Students.Remove(student);

            var lecturer = await _context.Lecturers.FindAsync(id);
            if (lecturer != null) _context.Lecturers.Remove(lecturer);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok();
        }


        // =====================================================
        // 2. QUẢN LÝ KHÓA HỌC (COURSE)
        // =====================================================

        [HttpGet("courses")]
        public async Task<IActionResult> GetCourses()
        {
            var courses = await _context.Courses
                .Select(c => new KhoaHocDto
                {
                    Id = c.CourseId,
                    MaMon = c.CourseCode,
                    TenMon = c.CourseName,
                    TinChi = c.Credit,
                    MoTa = c.Description
                })
                .OrderByDescending(x => x.Id) // Hiện môn mới nhất lên đầu
                .ToListAsync();

            return Ok(courses);
        }

        [HttpPost("courses")]
        public async Task<IActionResult> SaveCourse([FromBody] KhoaHocDto model)
        {
            if (string.IsNullOrWhiteSpace(model.MaMon) || string.IsNullOrWhiteSpace(model.TenMon))
            {
                return BadRequest("Vui lòng nhập Mã môn và Tên môn học.");
            }

            if (model.Id == 0)
            {
                // --- LOGIC THÊM MỚI ---
                if (await _context.Courses.AnyAsync(c => c.CourseCode == model.MaMon))
                {
                    return BadRequest($"Mã môn '{model.MaMon}' đã tồn tại.");
                }

                var course = new Course
                {
                    // KHÔNG gán CourseId để SQL tự tăng (Identity)
                    CourseCode = model.MaMon,
                    CourseName = model.TenMon,
                    Credit = model.TinChi > 0 ? model.TinChi : 3,
                    Description = model.MoTa
                };

                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Thêm môn học thành công!", id = course.CourseId });
            }
            else
            {
                // --- LOGIC CẬP NHẬT ---
                var existingCourse = await _context.Courses.FindAsync(model.Id);
                if (existingCourse == null) return NotFound("Không tìm thấy môn học.");

                existingCourse.CourseName = model.TenMon;
                existingCourse.Credit = model.TinChi;
                existingCourse.Description = model.MoTa;

                await _context.SaveChangesAsync();
                return Ok(new { message = "Cập nhật thành công!" });
            }
        }

        [HttpDelete("courses/{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // =====================================================
        // 3. QUẢN LÝ LỊCH HỌC (CLASS SCHEDULE)
        // =====================================================

        [HttpGet("schedules")]
        public async Task<IActionResult> GetSchedules()
        {
            var data = await (
                from s in _context.ClassSchedules
                join c in _context.Classes on s.ClassId equals c.ClassId
                join course in _context.Courses on c.CourseId equals course.CourseId
                join l in _context.Lecturers on c.LecturerId equals l.LecturerId into gl
                from lecturer in gl.DefaultIfEmpty()
                orderby s.ScheduleId descending
                select new ScheduleDto
                {
                    ScheduleId = s.ScheduleId,

                    MaKhoaHoc = course.CourseCode,
                    TenKhoaHoc = course.CourseName,
                    Credit = course.Credit,

                    MaLopHoc = c.ClassCode,
                    HocKy = c.Semester,

                    TenGiangVien = lecturer != null ? lecturer.FullName : "Chưa xếp GV",

                    NgayTrongTuan = s.DayOfWeek,
                    ThoiGianBatDau = s.StartTime,
                    ThoiGianKetThuc = s.EndTime,
                    PhongHoc = s.Room
                }
            ).ToListAsync();

            return Ok(data);
        }

        [HttpPost("schedules")]
        public async Task<IActionResult> CreateSchedule([FromBody] ScheduleDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cls = await _context.Classes
                .FirstOrDefaultAsync(x => x.ClassCode == model.MaLopHoc);

            if (cls == null)
                return BadRequest($"Không tìm thấy lớp {model.MaLopHoc}");

            if (!string.IsNullOrWhiteSpace(model.TenGiangVien))
            {
                var lecturer = await _context.Lecturers
                    .FirstOrDefaultAsync(x => x.FullName == model.TenGiangVien);

                if (lecturer != null)
                    cls.LecturerId = lecturer.LecturerId;
            }

            var schedule = new ClassSchedule
            {
                ClassId = cls.ClassId,
                DayOfWeek = model.NgayTrongTuan,
                StartTime = model.ThoiGianBatDau,
                EndTime = model.ThoiGianKetThuc,
                Room = model.PhongHoc
            };

            _context.ClassSchedules.Add(schedule);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Thêm lịch học thành công" });
        }

        [HttpPut("schedules/{id}")]
        public async Task<IActionResult> UpdateSchedule(int id, [FromBody] ScheduleDto model)
        {
            var schedule = await _context.ClassSchedules.FindAsync(id);
            if (schedule == null)
                return NotFound("Không tìm thấy lịch học");

            var cls = await _context.Classes
                .FirstOrDefaultAsync(x => x.ClassCode == model.MaLopHoc);

            if (cls == null)
                return BadRequest("Lớp học không tồn tại");

            schedule.ClassId = cls.ClassId;
            schedule.DayOfWeek = model.NgayTrongTuan;
            schedule.StartTime = model.ThoiGianBatDau;
            schedule.EndTime = model.ThoiGianKetThuc;
            schedule.Room = model.PhongHoc;

            // cập nhật giảng viên
            if (!string.IsNullOrWhiteSpace(model.TenGiangVien))
            {
                var lecturer = await _context.Lecturers
                    .FirstOrDefaultAsync(x => x.FullName == model.TenGiangVien);

                if (lecturer != null)
                    cls.LecturerId = lecturer.LecturerId;
            }

            await _context.SaveChangesAsync();
            return Ok("Cập nhật lịch học thành công");
        }

        [HttpDelete("schedules/{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var schedule = await _context.ClassSchedules.FindAsync(id);
            if (schedule == null) return NotFound();

            _context.ClassSchedules.Remove(schedule);
            await _context.SaveChangesAsync();
            return Ok();
        }
        // ===========================
        // 4. QUẢN LÝ THÔNG BÁO (ANNOUNCEMENT)
        // ===========================

        // GET: api/AdminApi/announcement
        [HttpGet("announcement")]
        public async Task<IActionResult> GetAnnouncements()
        {
            var announcements = await _context.Announcements
                .Include(a => a.Author)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new AnnouncementAdminDto
                {
                    AnnouncementId = a.AnnouncementId,
                    Title = a.Title,
                    Body = a.Body,
                    AuthorId = a.AuthorId,
                    AuthorName = a.Author != null ? a.Author.Username : "Unknown",
                    CreatedAt = a.CreatedAt,
                    IsGlobal = a.IsGlobal,
                    TargetClassId = a.TargetClassId
                })
                .ToListAsync();

            return Ok(announcements);
        }

        // GET: api/AdminApi/announcement/5
        [HttpGet("announcement/{id}")]
        public async Task<IActionResult> GetAnnouncement(int id)
        {
            var announcement = await _context.Announcements
                .Include(a => a.Author)
                .FirstOrDefaultAsync(a => a.AnnouncementId == id);

            if (announcement == null) return NotFound();

            var dto = new AnnouncementAdminDto
            {
                AnnouncementId = announcement.AnnouncementId,
                Title = announcement.Title,
                Body = announcement.Body,
                AuthorId = announcement.AuthorId,
                AuthorName = announcement.Author != null ? announcement.Author.Username : "Unknown",
                CreatedAt = announcement.CreatedAt,
                IsGlobal = announcement.IsGlobal,
                TargetClassId = announcement.TargetClassId
            };

            return Ok(dto);
        }

        // POST: api/AdminApi/announcement
        [HttpPost("announcement")]
        public async Task<IActionResult> CreateAnnouncement([FromBody] AnnouncementAdminDto model)
        {
            if (model == null) return BadRequest("Dữ liệu không hợp lệ");

            // Kiểm tra AuthorId, mặc định là Admin (1) nếu null
            int authorId = model.AuthorId ?? 1;

            var announcement = new Announcement
            {
                Title = model.Title,
                Body = model.Body,
                AuthorId = authorId,
                IsGlobal = model.IsGlobal,
                TargetClassId = model.TargetClassId,
                CreatedAt = DateTime.Now
            };

            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();

            // Trả về DTO
            var dto = new AnnouncementAdminDto
            {
                AnnouncementId = announcement.AnnouncementId,
                Title = announcement.Title,
                Body = announcement.Body,
                AuthorId = announcement.AuthorId,
                AuthorName = (await _context.Users.FindAsync(authorId))?.Username ?? "Unknown",
                CreatedAt = announcement.CreatedAt,
                IsGlobal = announcement.IsGlobal,
                TargetClassId = announcement.TargetClassId
            };

            return Ok(dto);
        }

        // PUT: api/AdminApi/announcement/5
        [HttpPut("announcement/{id}")]
        public async Task<IActionResult> UpdateAnnouncement(int id, [FromBody] AnnouncementAdminDto model)
        {
            if (model == null || id != model.AnnouncementId) return BadRequest();

            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null) return NotFound();

            announcement.Title = model.Title;
            announcement.Body = model.Body;
            announcement.IsGlobal = model.IsGlobal;
            announcement.TargetClassId = model.TargetClassId;
            announcement.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            var dto = new AnnouncementAdminDto
            {
                AnnouncementId = announcement.AnnouncementId,
                Title = announcement.Title,
                Body = announcement.Body,
                AuthorId = announcement.AuthorId,
                AuthorName = (await _context.Users.FindAsync(announcement.AuthorId))?.Username ?? "Unknown",
                CreatedAt = announcement.CreatedAt,
                IsGlobal = announcement.IsGlobal,
                TargetClassId = announcement.TargetClassId
            };

            return Ok(dto);
        }

        // DELETE: api/AdminApi/announcement/5
        [HttpDelete("announcement/{id}")]
        public async Task<IActionResult> DeleteAnnouncement(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null) return NotFound();

            _context.Announcements.Remove(announcement);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}