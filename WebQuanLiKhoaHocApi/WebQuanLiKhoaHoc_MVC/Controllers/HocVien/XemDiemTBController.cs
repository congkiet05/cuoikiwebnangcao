using Microsoft.AspNetCore.Mvc;
using WebQuanLiKhoaHoc_MVC.Models.HocVien;
using WebQuanLiKhoaHoc_MVC.Service;

namespace WebQuanLiKhoaHoc_MVC.Controllers.HocVien
{
    public class XemDiemTBController : Controller
    {
        private readonly HocVien_XemDiemTBService hocVien_XemDiemTBService;
        public XemDiemTBController(HocVien_XemDiemTBService hocVien_XemDiemTBService)
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

    }
}
