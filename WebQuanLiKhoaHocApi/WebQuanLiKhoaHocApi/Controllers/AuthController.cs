using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebQuanLiKhoaHocApi.Entities;
using WebQuanLiKhoaHocApi.Dtos;

namespace WebQuanLiKhoaHocApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UniversityDBContext _context;
        private readonly IConfiguration _config;

        public AuthController(UniversityDBContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // POST api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            // === BƯỚC A: TÌM USER VÀ KIỂM TRA PASSWORD ===

            // 1. Dùng .Include() để nạp "liên kết ảo" IdGroups
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == loginRequest.Username);

            // 2. Kiểm tra user & password (Dùng thuộc tính "Password" từ Model)
            // (Lưu ý: Đây là cách kiểm tra không an toàn, sẽ nói ở dưới)
            if (user == null || user.Password != loginRequest.Password)
            {
                return Unauthorized(new { message = "Tài khoản hoặc mật khẩu không hợp lệ." });
            }

            // === BƯỚC B: LẤY VAI TRÒ (ROLES) CỦA USER ===

            var role = user.Role.RoleName ?? "User"; // Giả sử mỗi user chỉ có một vai trò (role)

            // === BƯỚC C: TẠO TOKEN ===
            var token = GenerateJwtToken(user, role);

            // === BƯỚC D: TRẢ VỀ TOKEN VÀ ROLES CHO REACT ===
            return Ok(new LoginResponseDto
            {
                Token = token,
                Role = role
            });
        }

        // ==========================================
        // PHẦN MỚI THÊM: QUÊN MẬT KHẨU & RESET
        // ==========================================

        // POST api/Auth/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            // 1. Tìm user theo Email
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                return BadRequest(new { message = "Email không tồn tại trong hệ thống." });
            }

            // 2. Tạo Token ngẫu nhiên
            string token = Guid.NewGuid().ToString();

            // 3. Lưu Token và thời gian hết hạn (ví dụ 15 phút) vào DB
            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiry = DateTime.Now.AddMinutes(15);
            await _context.SaveChangesAsync();

            // 4. Gửi Token về client 
            // (Thực tế nên gửi qua Email, ở đây trả về API để bạn test cho dễ)
            return Ok(new
            {
                message = "Vui lòng kiểm tra email để lấy mã reset (đã giả lập trả về token bên dưới).",
                token = token // Dùng token này để test bước ResetPassword
            });
        }

        // POST api/Auth/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            // --- DEBUG LOG: Xem dữ liệu nhận được là gì ---
            Console.WriteLine($"[API Debug] Token received: {model.Token}");
            Console.WriteLine($"[API Debug] NewPass received: {model.NewPassword}");

            // 1. Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrEmpty(model.NewPassword))
            {
                return BadRequest(new { message = "Mật khẩu mới không được để trống." });
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                return BadRequest(new { message = "Mật khẩu xác nhận không khớp." });
            }

            // 2. Tìm user (Thêm AsTracking để chắc chắn EF theo dõi thay đổi)
            var user = await _context.Users
                .AsTracking() // <--- QUAN TRỌNG: Báo cho EF biết cần theo dõi object này
                .SingleOrDefaultAsync(u =>
                    u.PasswordResetToken == model.Token &&
                    u.PasswordResetTokenExpiry > DateTime.Now);

            if (user == null)
            {
                Console.WriteLine("[API Debug] Không tìm thấy user hoặc token hết hạn.");
                return BadRequest(new { message = "Mã xác nhận không hợp lệ hoặc đã hết hạn." });
            }

            // 3. Cập nhật mật khẩu
            // LƯU Ý: Nếu bạn nhập pass mới trùng pass cũ, dòng này sẽ không tạo ra lệnh SQL
            string oldPassword = user.Password;
            user.Password = model.NewPassword;

            // Xóa token
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;

            // 4. Ép buộc trạng thái Modified (để chắc chắn SQL chạy kể cả khi pass giống nhau)
            _context.Entry(user).State = EntityState.Modified;

            // 5. Lưu xuống DB
            try
            {
                await _context.SaveChangesAsync();
                Console.WriteLine($"[API Debug] Đã lưu thành công. User: {user.Username}, Pass cũ: {oldPassword} -> Mới: {user.Password}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API Debug] Lỗi SaveChanges: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi hệ thống khi lưu dữ liệu." });
            }

            return Ok(new { message = "Đổi mật khẩu thành công." });
        }
        
        // ==========================================
        // KẾT THÚC PHẦN MỚI THÊM
        // ==========================================


        // Hàm trợ giúp (helper) để TẠO TOKEN
        // (Dùng "Party" làm kiểu dữ liệu)
        private string GenerateJwtToken(User user, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Tạo "claims" (thông tin trong token)
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userId", user.UserId.ToString()) // Dùng thuộc tính "UserId"
            };

            claims.Add(new Claim(ClaimTypes.Role, role));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"],
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

    }
}