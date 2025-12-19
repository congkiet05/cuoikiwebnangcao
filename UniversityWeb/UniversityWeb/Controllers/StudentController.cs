using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using UniversityWeb.Models;

public class StudentController : Controller

{
    protected override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if (Session["Student"] == null)
        {
            filterContext.Result = RedirectToAction("Login", "Account");
        }
        base.OnActionExecuting(filterContext);
    }

    UniversityDBEntities1 db = new UniversityDBEntities1();

    private int GetStudentId()
    {
        Student sv = (Student)Session["Student"];
        return sv.StudentId;
    }

    public ActionResult Index()
    {
        return View();
    }

    // ================== ĐĂNG KÝ MÔN HỌC ==================
    public ActionResult DangKyMon()
    {
        var classes = db.Classes
                        .Include("Course")
                        .Include("Lecturer")
                        .ToList();
        return View(classes);
    }

    public ActionResult DangKy(int id)
    {
        int studentId = GetStudentId();

        if (!db.Registrations.Any(r => r.StudentId == studentId && r.ClassId == id))
        {
            db.Registrations.Add(new Registration
            {
                StudentId = studentId,
                ClassId = id,
                Status = "Registered"
            });
            db.SaveChanges();
        }

        return RedirectToAction("DangKyMon");
    }

    // ================== THAM GIA HỌC ==================
    public ActionResult ThamGiaHoc()
    {
        int studentId = GetStudentId();

        var data = db.Registrations
            .Where(r => r.StudentId == studentId)
            .Select(r => r.Class)
            .Include("Course")
            .ToList();

        return View(data);
    }

    // ================== THỜI KHÓA BIỂU ==================
    public ActionResult ThoiKhoaBieu()
    {
        int studentId = GetStudentId();

        var tkb = from r in db.Registrations
                  join cs in db.ClassSchedules on r.ClassId equals cs.ClassId
                  join c in db.Classes on r.ClassId equals c.ClassId
                  join course in db.Courses on c.CourseId equals course.CourseId
                  where r.StudentId == studentId
                  select new
                  {
                      course.CourseName,
                      cs.DayOfWeek,
                      cs.StartTime,
                      cs.EndTime,
                      cs.Room
                  };

        return View(tkb.ToList());
    }

    // ================== THÔNG BÁO ==================
    public ActionResult ThongBao()
    {
        int studentId = GetStudentId();

        var classIds = db.Registrations
            .Where(r => r.StudentId == studentId)
            .Select(r => r.ClassId);

        var tb = db.Announcements
            .Where(a => a.IsGlobal == true
                || (a.TargetClassId.HasValue && classIds.Contains(a.TargetClassId.Value)))
            .OrderByDescending(a => a.CreatedAt)
            .ToList();

        return View(tb);
    }
}
