using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PlantShop.Models;
using PlantShop.Services.VnPay;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<PlantShopDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddHttpClient();

builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

// Đăng ký dịch vụ Authentication (Cookie) - Dùng cho Khách hàng
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";   // Chưa đăng nhập thì về đây
        options.AccessDeniedPath = "/Account/AccessDenied"; // Không đủ quyền thì về đây
        options.ExpireTimeSpan = TimeSpan.FromDays(1); // Cookie tồn tại 1 ngày
    });

//connect vnpay API
builder.Services.AddScoped<IVnPayService, VnPayService>();

builder.Services.AddHttpContextAccessor();
var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseSession();

app.UseStaticFiles();// test
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
