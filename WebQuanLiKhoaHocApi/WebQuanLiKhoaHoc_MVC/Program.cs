using Microsoft.EntityFrameworkCore;
using WebQuanLiKhoaHoc_MVC.Service;
using WebQuanLiKhoaHocApi.Entities;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<UniversityDBContext>(options =>
    options.UseSqlServer(connectionString));

var apiBaseUrl = builder.Configuration.GetValue<String>("ApiSettings:BaseUrl");

// Đăng ký HttpClient dùng chung cho toàn bộ project
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// ... các cấu hình khác ...
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
    pattern: "{controller=Student}/{action=Dashboard}/{id?}"); //test giao diện Học viên
app.Run();
