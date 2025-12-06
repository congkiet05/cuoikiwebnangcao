using Microsoft.AspNetCore.Mvc;
using WebQuanLiKhoaHoc_MVC.Interface;

namespace WebQuanLiKhoaHoc_MVC.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Dashboard()
        {
            return View(); // -> Views/Student/Dashboard.cshtml
        }
    }
}
