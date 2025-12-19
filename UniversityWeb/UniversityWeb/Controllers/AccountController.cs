using System.Linq;
using System.Web.Mvc;
using UniversityWeb.Models;

public class AccountController : Controller
{
    UniversityDBEntities1 db = new UniversityDBEntities1();

    // GET: Login
    public ActionResult Login()
    {
        return View();
    }

    // POST: Login
    [HttpPost]
    public ActionResult Login(string username, string password)
    {
        var user = db.Users
                     .FirstOrDefault(u => u.Username == username
                                       && u.Password == password
                                       && u.Role.RoleName == "Student");

        if (user != null)
        {
            Session["Student"] = user.Student;
            return RedirectToAction("Index", "Student");
        }

        ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
        return View();
    }

    // Logout
    public ActionResult Logout()
    {
        Session.Clear();
        return RedirectToAction("Login");
    }
}
