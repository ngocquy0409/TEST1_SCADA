using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TEST1_SCADA.Migrations
{
    /// <inheritdoc />
    public partial class AddMonitorData2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MonitorDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenDayChuyen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenMay = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ca = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SanPhamId = table.Column<int>(type: "int", nullable: true),
                    MaSanPham = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenSanPham = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaTruongCa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenTruongCa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Speed = table.Column<int>(type: "int", nullable: false),
                    Oee = table.Column<int>(type: "int", nullable: false),
                    Stop5s = table.Column<int>(type: "int", nullable: false),
                    Stop5m = table.Column<int>(type: "int", nullable: false),
                    EmptyPct = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<int>(type: "int", nullable: false),
                    Good = table.Column<int>(type: "int", nullable: false),
                    Jam = table.Column<int>(type: "int", nullable: false),
                    Empty = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitorDatas", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonitorDatas");
        }
    }
}
