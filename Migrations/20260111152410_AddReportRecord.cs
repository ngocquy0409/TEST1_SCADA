using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TEST1_SCADA.Migrations
{
    /// <inheritdoc />
    public partial class AddReportRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReportRecords",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Line = table.Column<int>(type: "int", nullable: false),
                    Machine = table.Column<int>(type: "int", nullable: false),
                    CaSo = table.Column<int>(type: "int", nullable: false),
                    NgayBaoCao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MTTR = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MTBF = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StopPct = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FaultPct = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    A = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SpeedLossPct = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Vtb = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinorStopPct = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    P = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SpicePct = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EmptyPct = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Q = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OEE1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OEE2 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OEE3 = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportRecords", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportRecords");
        }
    }
}
