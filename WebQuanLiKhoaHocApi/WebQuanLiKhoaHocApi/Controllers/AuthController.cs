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
            if (user == null || user.PasswordHash != loginRequest.Password) 
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