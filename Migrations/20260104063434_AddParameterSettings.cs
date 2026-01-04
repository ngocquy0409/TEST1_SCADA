using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TEST1_SCADA.Migrations
{
    /// <inheritdoc />
    public partial class AddParameterSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParameterSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDayChuyen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenMay = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ca = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SanPhamId = table.Column<int>(type: "int", nullable: true),
                    MaSanPham = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenSanPham = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CaiDatThamSo = table.Column<bool>(type: "bit", nullable: false),
                    TocDoChuan = table.Column<int>(type: "int", nullable: true),
                    ThoiGianDungMay = table.Column<int>(type: "int", nullable: true),
                    ThoiGianChapNhanGoi = table.Column<int>(type: "int", nullable: true),
                    ThoiGianGoiCan = table.Column<int>(type: "int", nullable: true),
                    ThoiGianDayGoiCan = table.Column<int>(type: "int", nullable: true),
                    ThoiGianCapNhatTuPLC = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParameterSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParameterSettings");
        }
    }
}
