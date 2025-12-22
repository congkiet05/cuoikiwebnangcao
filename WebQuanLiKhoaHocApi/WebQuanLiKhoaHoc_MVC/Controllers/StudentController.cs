using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Formats.Asn1;
using System.Security.Claims;
using WebQuanLiKhoaHoc_MVC.Models;
using WebQuanLiKhoaHocApi.Entities;
namespace WebQuanLiKhoaHoc_MVC.Controllers
{
    [Authorize(Roles = "Student")]

    public class StudentController : Controller
    {
        private readonly UniversityDBContext _context;
        public IActionResult Dashboard()
        {
            return View("~/Views/Student/HocVien_Dashboard.cshtml"); // -> Views/Student/Dashboard.cshtml
        }
        public StudentController(UniversityDBContext context)
        {
            _context = context;
        }

        public IActionResult Message()
        {
            return View(); // -> Views/Student/Dashboard.cshtml
        }

    }
}
