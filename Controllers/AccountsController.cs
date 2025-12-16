using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TEST1_SCADA.Models;


namespace TEST1_SCADA.Controllers
{
    public class AccountsController : Controller
    {
        // Khai báo các biến cần thiết
        private readonly UserManager<IdentityUser> _userManager; // Quản lý người dùng
        private readonly SignInManager<IdentityUser> _signInManager; // Quản lý đăng nhập
        private readonly RoleManager<IdentityRole> _roleManager; // Quản lý vai trò
        public AccountsController(UserManager<IdentityUser> userManager,
                                  SignInManager<IdentityUser> signInManager,
                                  RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        } // Hàm khởi tạo để gán các biến
        // Hành động để xem, thêm, sửa và xóa vai trò

        public IActionResult ManageRoles() // Hiển thị danh sách vai trò
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }
        // Thêm vai trò mới
        [HttpPost] // Thêm vai trò mới vào hệ thống 
        public async Task<IActionResult> AddRole(string roleName) // Tên vai trò được truyền từ biểu mẫu
        {
            if (!string.IsNullOrEmpty(roleName) && !await _roleManager.RoleExistsAsync(roleName)) // Kiểm tra nếu tên vai trò không rỗng và vai trò chưa tồn tại
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));  // Thêm vai trò vào hệ thống
            }
            return RedirectToAction("ManageRoles"); // Quay lại trang quản lý vai trò
        }

        // Xóa vai trò
        [HttpPost] // Xóa vai trò khỏi hệ thống
        public async Task<IActionResult> DeleteRole(string roleId) // ID vai trò được truyền từ biểu mẫu
        {
            var role = await _roleManager.FindByIdAsync(roleId); // Tìm vai trò theo ID
            if (role != null)
            {
                await _roleManager.DeleteAsync(role); // Xóa vai trò khỏi hệ thống
            }
            return RedirectToAction("ManageRoles"); // Quay lại trang quản lý vai trò
        }

        // Sửa vai trò
        [HttpPost] // Cập nhật tên vai trò
        public async Task<IActionResult> EditRole(string roleId, string roleName) // ID vai trò và tên mới được truyền từ biểu mẫu
        {
            var role = await _roleManager.FindByIdAsync(roleId); // Tìm vai trò theo ID
            if (role != null) // Kiểm tra nếu vai trò tồn tại và tên mới không rỗng
            {
                role.Name = roleName; // Cập nhật tên vai trò
                await _roleManager.UpdateAsync(role); // Lưu thay đổi vào hệ thống
            }
            return RedirectToAction("ManageRoles"); // Quay lại trang quản lý vai trò
        }

        // Đăng ký người dùng với lựa chọn role
        [HttpGet] // Hiển thị biểu mẫu đăng ký
        public IActionResult Register()
        {
            ViewBag.Roles = new List<string> { "Admin", "Operator" };
            return View();
        }


        // Đăng ký người dùng 
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new List<string> { "Admin", "Operator" };
                return View(model);
            }

            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            ViewBag.Roles = new List<string> { "Admin", "Operator" };
            return View(model);
        }


        // Action cho trang đăng nhập
        [HttpGet]
        public IActionResult Login()
        {
            return View(); // Trả về biểu mẫu đăng nhập
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)    // Xử lý đăng nhập người dùng
        {
            if (ModelState.IsValid) // Kiểm tra nếu mô hình hợp lệ
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false); // Đăng nhập người dùng
                if (result.Succeeded) //    Nếu đăng nhập thành công
                {
                    return RedirectToAction("Index", "Products"); // Chuyển hướng đến trang chủ
                }
                ModelState.AddModelError(string.Empty, "Đăng nhập không thành công."); // Thêm lỗi vào trạng thái mô hình nếu đăng nhập thất bại
            }
            return View(model); // Trả về biểu mẫu đăng nhập với mô hình
        }

        // Action đăng xuất
        [HttpPost]
        public async Task<IActionResult> Logout() // Xử lý đăng xuất người dùng
        {
            await _signInManager.SignOutAsync(); // Đăng xuất người dùng
            return RedirectToAction("Index", "Products"); // Chuyển hướng đến trang chủ
        }
        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);

            var result = await _userManager.ChangePasswordAsync(
                user,
                model.OldPassword,
                model.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                ViewBag.Success = "Đổi mật khẩu thành công";
                return View();
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }
    }
}
