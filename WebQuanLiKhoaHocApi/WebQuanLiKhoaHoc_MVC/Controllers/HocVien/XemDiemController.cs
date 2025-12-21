using Microsoft.AspNetCore.Mvc;
using WebQuanLiKhoaHoc_MVC.Models.HocVien;
using WebQuanLiKhoaHoc_MVC.Service;

namespace WebQuanLiKhoaHoc_MVC.Controllers.HocVien
{
    public class XemDiemController : Controller
    {
        private readonly HocVien_XemDiemService hocVien_XemDiemTBService;
        
        public XemDiemController(HocVien_XemDiemService hocVien_XemDiemTBService)
        {
            this.hocVien_XemDiemTBService = hocVien_XemDiemTBService;

        }
        public async Task<IActionResult> XemDiemTrungBinh(string MaHocVien)
        {
            string viewPath = "~/Views/Student/XemDiemTb.cshtml";

            if (string.IsNullOrEmpty(MaHocVien))
            {

                return View(viewPath, new List<HocVien_XemDiemTBModels>());
            }

            var xemDiemTrungBinhs = await hocVien_XemDiemTBService.XemDiemTrungBinh(MaHocVien);
            ViewBag.CurrentFilter = MaHocVien;

            return View(viewPath, xemDiemTrungBinhs);
        }
        public async Task<IActionResult> XemDiemMon(string MaHocVien , string MaHocPhan)
        {
            string viewPath = "~/Views/Student/XemDiemChiTiet.cshtml";

            if (string.IsNullOrEmpty(MaHocVien))
            {

                return View(viewPath, new List<HocVien_XemDiemMonModels>());
            }

            var xemDiemMon = await hocVien_XemDiemTBService.XemDiemMonHoc(MaHocVien , MaHocPhan);
            ViewBag.CurrentFilter = MaHocVien;

            return View(viewPath, xemDiemMon);
        }
    }
}
