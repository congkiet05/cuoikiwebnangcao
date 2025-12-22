using System.ComponentModel.DataAnnotations;

namespace WebQuanLiKhoaHoc_MVC.Models.Login
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress]
        public string Email { get; set; }
    }
}

