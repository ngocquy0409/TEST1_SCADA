using ClosedXML.Excel;      // thư viện tạo file Excel
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using S7.Net;
using TEST1_SCADA.Data;
using TEST1_SCADA.Models;
using DocumentFormat.OpenXml;               // thư viện tạo file Word
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;    // thư viện tạo file Word

namespace TEST1_SCADA.Controllers
{
    public class ReportController : Controller
    {
        private static Plc _plc = new Plc(CpuType.S71200, "192.168.56.1", 0, 1);    // PLC instance dùng chung
        private readonly ApplicationDbContext _db;          // thêm ApplicationDbContext để truy cập DB 
        private readonly ILogger<ReportController> _logger;    // thêm logger để ghi log

        public ReportController(ILogger<ReportController> logger, ApplicationDbContext db)
        {
            _logger = logger;   // khởi tạo biến _logger 
            _db = db;           // khởi tạo biến _db 
        }

        public IActionResult Index() => View(); // trang index đơn giản

        private void EnsureConnected() // đảm bảo kết nối đến PLC
        {
            if (_plc == null)
                _plc = new Plc(CpuType.S71200, "192.168.56.1", 0, 1);

            if (!_plc.IsConnected)
                _plc.Open();
        }

        // POST /report/connect
        [HttpPost("connect")] // kết nối đến PLC
        public IActionResult Connect()
        {
            try
            {
                if (_plc == null || !_plc.IsConnected)
                {
                    return Json(new { ok = false, message = "Chưa kết nối đến PLC!" });
                }
                EnsureConnected();
                return Json(new { success = _plc.IsConnected, status = _plc.IsConnected ? "Kết nối đến PLC thành công!" : "Kết nối đến PLC thất bại!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi kết nối đến PLC");
                return StatusCode(500, new { success = false, status = $"Lỗi khi kết nối đến PLC: {ex.Message}" });
            }
        }

        private int ReadDbInt(int db, int dbwOffset)
        {
            var obj = _plc!.Read($"DB{db}.DBW{dbwOffset}");
            if (obj is short s) return s;
            if (obj is ushort us) return us;
            return Convert.ToInt32(obj);
        }

        private decimal ReadDbDec10(int db, int dbwOffset)
        {
            // PLC lưu giá trị
            return ReadDbInt(db, dbwOffset); // có thể chia 10m nếu cần
        }

        private int GetReportDbByMachine(int machine) => machine switch
        {
            1 => 8,
            2 => 9,
            3 => 10,
            4 => 11,
            _ => 8
        };

        // API đọc PLC + lưu DB
        [HttpGet]
        public async Task<IActionResult> ReadSave([FromQuery] int line = 1, [FromQuery] int machine = 1, [FromQuery] int caSo = 1)
        {
            try
            {
                EnsureConnected();

                var db = GetReportDbByMachine(machine);

                // Offset ví dụ (bạn sửa theo DB thật)
                var rec = new ReportRecord
                {
                    Line = line,
                    Machine = machine,
                    CaSo = caSo,
                    CreatedAt = DateTime.Now,

                    MTTR = ReadDbDec10(db, 0),
                    MTBF = ReadDbDec10(db, 2),
                    StopPct = ReadDbDec10(db, 4),
                    FaultPct = ReadDbDec10(db, 6),
                    A = ReadDbDec10(db, 8),

                    SpeedLossPct = ReadDbDec10(db, 10),
                    Vtb = ReadDbInt(db, 12),
                    MinorStopPct = ReadDbDec10(db, 14),
                    P = ReadDbDec10(db, 16),

                    SpicePct = ReadDbDec10(db, 18),
                    EmptyPct = ReadDbDec10(db, 20),
                    Q = ReadDbDec10(db, 22),

                    OEE1 = ReadDbDec10(db, 24),
                    OEE2 = ReadDbDec10(db, 26),
                    OEE3 = ReadDbDec10(db, 28),
                };

                _db.ReportRecords.Add(rec);
                await _db.SaveChangesAsync();

                return Json(new { ok = true, updatedAt = DateTime.Now.ToString("HH:mm:ss"), data = rec });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Report ReadSave failed");
                return Json(new { ok = false, message = ex.Message });
            }
        }

        // Lấy bản ghi mới nhất theo bộ lọc (dùng chung)
        private async Task<ReportRecord?> GetLatestReportAsync(int line, int machine, int caSo)
        {
            return await _db.ReportRecords
                .Where(x => x.Line == line && x.Machine == machine && x.CaSo == caSo)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcel(int line = 1, int machine = 1, int caSo = 1)
        {
            var rec = await GetLatestReportAsync(line, machine, caSo);
            if (rec == null) return BadRequest("Chưa có dữ liệu báo cáo để xuất!");

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("BaoCao");

            // Title
            ws.Cell("A1").Value = "BẢNG THÔNG SỐ ĐÁNH GIÁ";
            ws.Range("A1:C1").Merge().Style.Font.SetBold().Font.SetFontSize(16);
            ws.Range("A1:C1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Cell("A2").Value = $"Dây chuyền: {line}";
            ws.Cell("B2").Value = $"Máy: {machine}";
            ws.Cell("C2").Value = $"Ca: {caSo}";

            ws.Cell("A4").Value = "Nhóm";
            ws.Cell("B4").Value = "Chỉ số";
            ws.Cell("C4").Value = "Giá trị";

            ws.Range("A4:C4").Style.Font.SetBold();
            ws.Range("A4:C4").Style.Fill.SetBackgroundColor(XLColor.LightGray);

            int r = 5;

            void Row(string group, string name, object? val)
            {
                ws.Cell(r, 1).Value = group;
                ws.Cell(r, 2).Value = name;
                ws.Cell(r, 3).Value = val?.ToString() ?? "";
                r++;
            }

            // SẴN SÀNG
            Row("SẴN SÀNG", "MTTR [phút]", rec.MTTR);
            Row("SẴN SÀNG", "MTBF [giờ]", rec.MTBF);
            Row("SẴN SÀNG", "% Dừng máy [%]", rec.StopPct);
            Row("SẴN SÀNG", "% Hỏng máy [%]", rec.FaultPct);
            Row("SẴN SÀNG", "Availability Rate [%]", rec.A);

            // ỔN ĐỊNH
            Row("ỔN ĐỊNH", "Tổn thất tốc độ [%]", rec.SpeedLossPct);
            Row("ỔN ĐỊNH", "Tốc độ trung bình [gói/phút]", rec.Vtb);
            Row("ỔN ĐỊNH", "% Thời gian dừng nhỏ [%]", rec.MinorStopPct);
            Row("ỔN ĐỊNH", "Performance Rate [%]", rec.P);

            // CHẤT LƯỢNG
            Row("CHẤT LƯỢNG", "% Gói cấn gia vị [%]", rec.SpicePct);
            Row("CHẤT LƯỢNG", "% Gói rỗng [%]", rec.EmptyPct);
            Row("CHẤT LƯỢNG", "Quality Rate [%]", rec.Q);

            // HIỆU SUẤT
            Row("HIỆU SUẤT", "% OEE 1 [%]", rec.OEE1);
            Row("HIỆU SUẤT", "% OEE 2 [%]", rec.OEE2);
            Row("HIỆU SUẤT", "% OEE 3 [%]", rec.OEE3);

            // Format
            ws.Columns().AdjustToContents();
            ws.Range($"A4:C{r - 1}").Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            ws.Range($"A4:C{r - 1}").Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
            ws.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            var bytes = ms.ToArray();

            var fileName = $"BaoCao_Line{line}_May{machine}_Ca{caSo}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
        // Xuất Word 
        [HttpGet]
        public IActionResult ExportWord(int line = 1, int machine = 1, int caSo = 1)
        {
            // TODO: lấy data báo cáo từ DB (ReportRecord) hoặc PLC
            // ví dụ: var d = ... (mttr, mtbf, stopPct,...)

            using var ms = new MemoryStream();
            using (var doc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document, true))
            {
                var main = doc.AddMainDocumentPart();
                main.Document = new Document(new Body());
                var body = main.Document.Body!;

                // Title
                body.AppendChild(new Paragraph(
                    new ParagraphProperties(new Justification() { Val = JustificationValues.Center }),
                    new Run(new RunProperties(new Bold(), new FontSize { Val = "32" }), new Text("BẢNG THÔNG SỐ ĐÁNH GIÁ"))
                ));

                body.AppendChild(new Paragraph(new Run(new Text($"Dây chuyền: {line}    Máy: {machine}    Ca: {caSo}"))));
                body.AppendChild(new Paragraph(new Run(new Text($"Cập nhật: {DateTime.Now:dd/MM/yyyy HH:mm:ss}"))));
                body.AppendChild(new Paragraph(new Run(new Text(" "))));

                // Table 3 cột: Nhóm | Chỉ số | Giá trị
                var table = new Table();

                var props = new TableProperties(
                    new TableBorders(
                        new TopBorder { Val = BorderValues.Single, Size = 8 },
                        new BottomBorder { Val = BorderValues.Single, Size = 8 },
                        new LeftBorder { Val = BorderValues.Single, Size = 8 },
                        new RightBorder { Val = BorderValues.Single, Size = 8 },
                        new InsideHorizontalBorder { Val = BorderValues.Single, Size = 8 },
                        new InsideVerticalBorder { Val = BorderValues.Single, Size = 8 }
                    )
                );
                table.AppendChild(props);

                TableRow Row(string g, string name, string val)
                {
                    TableCell Cell(string t) => new TableCell(new Paragraph(new Run(new Text(t ?? ""))));
                    return new TableRow(Cell(g), Cell(name), Cell(val));
                }

                // Header
                table.AppendChild(Row("Nhóm", "Chỉ số", "Giá trị"));

                // Ví dụ map dữ liệu (bạn thay bằng data thật)
                string V(object? x) => x?.ToString() ?? "";

                // SẴN SÀNG
                table.AppendChild(Row("SẴN SÀNG", "MTTR [phút]", V(30)));
                table.AppendChild(Row("SẴN SÀNG", "MTBF [giờ]", V(20)));
                table.AppendChild(Row("SẴN SÀNG", "% Dừng máy [%]", V(30)));
                table.AppendChild(Row("SẴN SÀNG", "% Hỏng máy [%]", V(40)));
                table.AppendChild(Row("SẴN SÀNG", "Availability Rate [%]", V(45)));

                // ỔN ĐỊNH
                table.AppendChild(Row("ỔN ĐỊNH", "Tổn thất tốc độ [%]", V(46)));
                table.AppendChild(Row("ỔN ĐỊNH", "Tốc độ trung bình [gói/phút]", V(57)));
                table.AppendChild(Row("ỔN ĐỊNH", "% Thời gian dừng nhỏ [%]", V(70)));
                table.AppendChild(Row("ỔN ĐỊNH", "Performance Rate [%]", V(60)));

                // CHẤT LƯỢNG
                table.AppendChild(Row("CHẤT LƯỢNG", "% Gói cấn gia vị [%]", V(0)));
                table.AppendChild(Row("CHẤT LƯỢNG", "% Gói rỗng [%]", V(80)));
                table.AppendChild(Row("CHẤT LƯỢNG", "Quality Rate [%]", V(0)));

                // HIỆU SUẤT
                table.AppendChild(Row("HIỆU SUẤT", "% OEE 1 [%]", V(67)));
                table.AppendChild(Row("HIỆU SUẤT", "% OEE 2 [%]", V(500)));
                table.AppendChild(Row("HIỆU SUẤT", "% OEE 3 [%]", V(68)));

                body.AppendChild(table);

                main.Document.Save();
            }

            var bytes = ms.ToArray();
            var fileName = $"BaoCao_PLC_Line{line}_May{machine}_Ca{caSo}_{DateTime.Now:yyyyMMdd_HHmmss}.docx";
            return File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
        }
    }
}

