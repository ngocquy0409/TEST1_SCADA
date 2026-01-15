using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using TEST1_SCADA.Models;

namespace TEST1_SCADA.Data
{
    public class ApplicationDbContext : IdentityDbContext // dùng cho việc đăng nhập và phân quyền
    {
        // constructor 
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        // constructor
        public DbSet<TruongCa> TruongCa { get; set; }   // bảng trưởng ca 

        public DbSet<SanPham> SanPham { get; set; }     // bảng sản phẩm

        public DbSet<ShiftConfig> ShiftConfigs { get; set; } = default!;        // bảng cấu hình ca

        public DbSet<ProductionInput> ProductionInputs { get; set; } = default!;   //bảng lưu dữ liệu người dùng nhập vào trên trang cài đặt sản xuất

        public DbSet<ParameterSetting> ParameterSettings { get; set; } // bảng lưu thông số người dùng nhập vào

        public DbSet<MonitorRecord> MonitorRecords { get; set; } = default!; // bảng lưu dữ liệu giám sát từ PLC
        public DbSet<ReportRecord> ReportRecords { get; set; }  // bảng lưu dữ liệu báo cáo hàng ngày 


    }
}
