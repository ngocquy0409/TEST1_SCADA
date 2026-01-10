using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TEST1_SCADA.Migrations
{
    /// <inheritdoc />
    public partial class AddMonitorRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "MonitorDatas");

            migrationBuilder.CreateTable(
                name: "MonitorRecords",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Line = table.Column<int>(type: "int", nullable: false),
                    Machine = table.Column<int>(type: "int", nullable: false),
                    CaSo = table.Column<int>(type: "int", nullable: false),
                    PollMs = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TruongCaId = table.Column<int>(type: "int", nullable: true),
                    SanPhamId = table.Column<int>(type: "int", nullable: true),
                    M1_Speed = table.Column<int>(type: "int", nullable: true),
                    M1_Oee = table.Column<int>(type: "int", nullable: true),
                    M1_Stop5s = table.Column<int>(type: "int", nullable: true),
                    M1_Stop5m = table.Column<int>(type: "int", nullable: true),
                    M1_EmptyPct = table.Column<int>(type: "int", nullable: true),
                    M1_Total = table.Column<int>(type: "int", nullable: true),
                    M1_Good = table.Column<int>(type: "int", nullable: true),
                    M1_Jam = table.Column<int>(type: "int", nullable: true),
                    M1_Empty = table.Column<int>(type: "int", nullable: true),
                    M2_Speed = table.Column<int>(type: "int", nullable: true),
                    M2_Oee = table.Column<int>(type: "int", nullable: true),
                    M2_Stop5s = table.Column<int>(type: "int", nullable: true),
                    M2_Stop5m = table.Column<int>(type: "int", nullable: true),
                    M2_EmptyPct = table.Column<int>(type: "int", nullable: true),
                    M2_Total = table.Column<int>(type: "int", nullable: true),
                    M2_Good = table.Column<int>(type: "int", nullable: true),
                    M2_Jam = table.Column<int>(type: "int", nullable: true),
                    M2_Empty = table.Column<int>(type: "int", nullable: true),
                    M3_Speed = table.Column<int>(type: "int", nullable: true),
                    M3_Oee = table.Column<int>(type: "int", nullable: true),
                    M3_Stop5s = table.Column<int>(type: "int", nullable: true),
                    M3_Stop5m = table.Column<int>(type: "int", nullable: true),
                    M3_EmptyPct = table.Column<int>(type: "int", nullable: true),
                    M3_Total = table.Column<int>(type: "int", nullable: true),
                    M3_Good = table.Column<int>(type: "int", nullable: true),
                    M3_Jam = table.Column<int>(type: "int", nullable: true),
                    M3_Empty = table.Column<int>(type: "int", nullable: true),
                    M4_Speed = table.Column<int>(type: "int", nullable: true),
                    M4_Oee = table.Column<int>(type: "int", nullable: true),
                    M4_Stop5s = table.Column<int>(type: "int", nullable: true),
                    M4_Stop5m = table.Column<int>(type: "int", nullable: true),
                    M4_EmptyPct = table.Column<int>(type: "int", nullable: true),
                    M4_Total = table.Column<int>(type: "int", nullable: true),
                    M4_Good = table.Column<int>(type: "int", nullable: true),
                    M4_Jam = table.Column<int>(type: "int", nullable: true),
                    M4_Empty = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitorRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonitorRecords_SanPham_SanPhamId",
                        column: x => x.SanPhamId,
                        principalTable: "SanPham",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MonitorRecords_TruongCa_TruongCaId",
                        column: x => x.TruongCaId,
                        principalTable: "TruongCa",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MonitorRecords_SanPhamId",
                table: "MonitorRecords",
                column: "SanPhamId");

            migrationBuilder.CreateIndex(
                name: "IX_MonitorRecords_TruongCaId",
                table: "MonitorRecords",
                column: "TruongCaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonitorRecords");

            migrationBuilder.CreateTable(
                name: "MonitorDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ca = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Empty = table.Column<int>(type: "int", nullable: false),
                    EmptyPct = table.Column<int>(type: "int", nullable: false),
                    Good = table.Column<int>(type: "int", nullable: false),
                    Jam = table.Column<int>(type: "int", nullable: false),
                    MaSanPham = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaTruongCa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Oee = table.Column<int>(type: "int", nullable: false),
                    SanPhamId = table.Column<int>(type: "int", nullable: true),
                    Speed = table.Column<int>(type: "int", nullable: false),
                    Stop5m = table.Column<int>(type: "int", nullable: false),
                    Stop5s = table.Column<int>(type: "int", nullable: false),
                    TenDayChuyen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenMay = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenSanPham = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenTruongCa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Total = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitorDatas", x => x.Id);
                });
        }
    }
}
