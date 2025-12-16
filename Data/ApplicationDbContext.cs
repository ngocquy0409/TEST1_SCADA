using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using TEST1_SCADA.Models;

namespace TEST1_SCADA.Data
{
    public class ApplicationDbContext : IdentityDbContext // dùng cho việc đăng nhập và phân quyền
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Product { get; set; }

        public DbSet<TruongCa> TruongCa { get; set; }

        public DbSet<SanPham> SanPham { get; set; }

    }
}
