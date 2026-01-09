using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TEST1_SCADA.Migrations
{
    /// <inheritdoc />
    public partial class AddMonitorData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MonitorData",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DayChuyen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    May = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ca = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaTruongCa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaSanPham = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TocDo = table.Column<int>(type: "int", nullable: false),
                    OEE = table.Column<float>(type: "real", nullable: false),
                    Stop5s = table.Column<int>(type: "int", nullable: false),
                    Stop5m = table.Column<int>(type: "int", nullable: false),
                    EmptyPct = table.Column<float>(type: "real", nullable: false),
                    Total = table.Column<int>(type: "int", nullable: false),
                    Good = table.Column<int>(type: "int", nullable: false),
                    Jam = table.Column<int>(type: "int", nullable: false),
                    Empty = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitorData", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonitorData");
        }
    }
}
