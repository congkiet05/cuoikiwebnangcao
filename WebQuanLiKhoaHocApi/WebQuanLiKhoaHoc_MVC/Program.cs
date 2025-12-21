using WebQuanLiKhoaHoc_MVC.Interface;
using WebQuanLiKhoaHoc_MVC.Service;

var builder = WebApplication.CreateBuilder(args);

// 1. MVC
builder.Services.AddControllersWithViews();

// 2. HttpClient gọi API - ĐẢM BẢO PORT PHẢI KHỚP VỚI PROJECT API
builder.Services.AddHttpClient("ApiGeneric", client =>
{
    // Thay 7274 bằng Port của API (ví dụ 5204 hoặc 7137)
    client.BaseAddress = new Uri("https://localhost:7274/api/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// 3. Đăng ký Services để sử dụng Dependency Injection
builder.Services.AddScoped<IScheduleApiService, ScheduleApiService>();
builder.Services.AddScoped<ApiService>();

var app = builder.Build();

// Pipeline cấu hình thứ tự xử lý request
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Nếu bạn có dùng Cookie/Session để lưu JWT Token, hãy thêm UseAuthentication() ở đây
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Dashboard}/{id?}");

app.Run();