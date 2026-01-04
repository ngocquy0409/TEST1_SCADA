using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TEST1_SCADA.Migrations
{
    /// <inheritdoc />
    public partial class AddProductionInput : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductionInputs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgaySanXuat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SanPhamId = table.Column<int>(type: "int", nullable: false),
                    CaSo = table.Column<int>(type: "int", nullable: false),
                    DayChuyen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SanLuongThuc = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionInputs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductionInputs");
        }
    }
}
