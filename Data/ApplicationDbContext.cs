using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using TEST1_SCADA.Models;

namespace TEST1_SCADA.Data
{
    public class ApplicationDbContext : IdentityDbContext // dùng cho việc đăng nhập và phân quyền
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<TruongCa> TruongCa { get; set; } // bảng trưởng ca 

        public DbSet<SanPham> SanPham { get; set; }
        public DbSet<ShiftConfig> ShiftConfigs { get; set; } = default!;

        public DbSet<ProductionInput> ProductionInputs { get; set; } = default!;

        public DbSet<ParameterSetting> ParameterSettings { get; set; } // bảng lưu thông số người dùng nhập vào
    }
}
