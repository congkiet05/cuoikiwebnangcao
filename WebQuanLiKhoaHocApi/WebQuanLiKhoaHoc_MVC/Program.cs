using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebQuanLiKhoaHoc_MVC.Interface;
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
        options.LoginPath = "/Login/Login"; // Nếu chưa đăng nhập thì tự chuyển về đây
        options.AccessDeniedPath = "/Account/AccessDenied"; // Không có quyền thì chuyển về đây
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });
builder.Services.AddHttpClient("ApiGeneric", client =>
{
    // Thay 7274 bằng Port của API (ví dụ 5204 hoặc 7137)
    client.BaseAddress = new Uri("https://localhost:7274/api/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<HoSoHocVienService>();
builder.Services.AddHttpClient<HocVienLichHoc>();
builder.Services.AddHttpClient<HocVien_XemDiemService>();
builder.Services.AddHttpClient<BaiTapService>();

builder.Services.AddScoped<IScheduleApiService, ScheduleApiService>();
builder.Services.AddScoped<ApiService>();
builder.Services.AddHttpClient<HocVien_thongbao>()
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        return new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        };
    });

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
