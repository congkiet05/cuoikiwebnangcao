using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebQuanLiKhoaHoc_MVC.Service;
using WebQuanLiKhoaHoc_MVC.Service.Login;
using WebQuanLiKhoaHocApi.Entities;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<UniversityDBContext>(options =>
    options.UseSqlServer(connectionString));

// Huu Thuan - Dang Ki Su Dung Cookie Authentication
builder.Services.AddHttpClient<AuthApiService>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Nếu chưa đăng nhập thì tự chuyển về đây
        options.AccessDeniedPath = "/Account/AccessDenied"; // Không có quyền thì chuyển về đây
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<HoSoHocVienService>();
builder.Services.AddHttpClient<HocVienLichHoc>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    //pattern: "{controller=Home}/{action=Index}/{id?}");
    pattern: "{controller=Login}/{action=Login}/{id?}"); //test giao diện Học viên
app.Run();
