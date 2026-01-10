using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Identity.UI; // cần thêm gói này để đảm bảo hoạt động đúng chức năng phân quyền
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using TEST1_SCADA.Data;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Cấu hình để kết nối cơ sở dữ liệu
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Chức năng đăng nhập, đăng ký và phân quyền người dùng
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // Tắt các yêu cầu về mật khẩu phức tạp
    options.Password.RequireDigit = false; // Không yêu cầu chữ số
    options.Password.RequireLowercase = false; // Không yêu cầu chữ thường
    options.Password.RequireUppercase = false; // Không yêu cầu chữ hoa
    options.Password.RequireNonAlphanumeric = false; // Không yêu cầu ký tự đặc biệt
    options.Password.RequiredLength = 1; // Độ dài tối thiểu của mật khẩu là 1 kí tự
    options.Password.RequiredUniqueChars = 0; // không yêu cầu ký tự duy nhất
})
.AddRoles<IdentityRole>() // Thêm hỗ trợ cho vai trò người dùng
.AddEntityFrameworkStores<ApplicationDbContext>(); // Sử dụng ApplicationDbContext để lưu trữ thông tin người dùng

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Accounts/Login";
    options.LogoutPath = "/Accounts/Logout";
    options.AccessDeniedPath = "/Accounts/Login";
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Admin", "Operator" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}
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

app.UseAuthentication();

app.UseAuthorization();

//app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}");

app.Run();
