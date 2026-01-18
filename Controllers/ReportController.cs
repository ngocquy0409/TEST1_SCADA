using ClosedXML.Excel;      // thư viện tạo file Excel
using DocumentFormat.OpenXml;               // thư viện tạo file Word
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;    // thư viện tạo file Word
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using S7.Net;
using TEST1_SCADA.Data;
using TEST1_SCADA.Models;

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
                // đọc dữ liệu từ PLC
                var db = GetReportDbByMachine(machine);
                var rec = new ReportRecord              // tạo bản ghi mới
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
                // lưu bản ghi vào DB
                _db.ReportRecords.Add(rec);
                await _db.SaveChangesAsync(); // lưu thay đổi vào DB
                // trả về kết quả dưới dạng JSON 
                return Json(new { ok = true, updatedAt = DateTime.Now.ToString("HH:mm:ss"), data = rec });
            }
            // bắt lỗi và ghi log
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
            var rec = await GetLatestReportAsync(line, machine, caSo);  // lấy bản ghi mới nhất theo bộ lọc 
            if (rec == null) return BadRequest("Chưa có dữ liệu báo cáo để xuất!"); // nếu không có dữ liệu thì báo lỗi

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
            // trả về file Excel đã tạo
            var fileName = $"BaoCao_Line{line}_May{machine}_Ca{caSo}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
        // Xuất Word 
        [HttpGet]
        public async Task<IActionResult> ExportWord(int line = 1, int machine = 1, int caSo = 1)
        {
            // lấy bản ghi mới nhất theo bộ lọc
            var rec = await GetLatestReportAsync(line, machine, caSo);
            if (rec == null) return BadRequest("Chưa có dữ liệu báo cáo để in!");
            // hàm chuyển đổi giá trị thành chuỗi
            string V(object? x) => x?.ToString() ?? "--";
            // tạo file Word trong bộ nhớ
            using var ms = new MemoryStream();
            using (var doc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document, true))
            {
                // tạo phần chính của tài liệu
                var main = doc.AddMainDocumentPart();
                main.Document = new Document(new Body());
                var body = main.Document.Body!;

                // tạo đoạn văn bản 
                Paragraph Para(string text,
                    bool bold = false,
                    int fontSize = 24, // 24=12pt
                    JustificationValues? just = null,
                    int leftIndentTwips = 0)
                {
                    // tạo đoạn văn bản với các thuộc tính định dạng
                    var p = new Paragraph();
                    var pPr = new ParagraphProperties();
                    pPr.Justification = new Justification { Val = just ?? JustificationValues.Left };
                    if (leftIndentTwips > 0)
                        pPr.Indentation = new Indentation { Left = leftIndentTwips.ToString() };
                    p.Append(pPr);
                    // tạo thuộc tính chạy văn bản
                    var rPr = new RunProperties();
                    if (bold) rPr.Append(new Bold());
                    rPr.Append(new FontSize { Val = fontSize.ToString() });
                    // thêm văn bản vào đoạn
                    p.Append(new Run(rPr, new Text(text) { Space = SpaceProcessingModeValues.Preserve }));
                    return p;
                }
                // tạo dòng trống
                void Blank() => body.Append(Para(" "));
                // tạo dòng gạch đầu dòng
                void Bullet(string label, object? val, string suffix = "")
                    => body.Append(Para($"-    {label}: {V(val)}{suffix}", leftIndentTwips: 720));
                // tạo ghi chú
                void Note(string text)
                {
                    body.Append(Para($"⇒  {text}", leftIndentTwips: 900));
                    Blank();
                }
                // tạo nội dung báo cáo
                body.Append(Para("CÔNG TY TNHH MỘT THÀNH VIÊN MASAN HẢI DƯƠNG", bold: true, fontSize: 32, just: JustificationValues.Center));
                body.Append(Para("BẢNG THÔNG SỐ ĐÁNH GIÁ", bold: true, fontSize: 32, just: JustificationValues.Center));
                Blank();

                body.Append(Para($"Dây chuyền: {line}    Máy: {machine}    Ca: {caSo}", fontSize: 24));
                body.Append(Para($"Cập nhật: {DateTime.Now:dd/MM/yyyy HH:mm:ss}", fontSize: 24));
                Blank();

                // SẴN SÀNG
                body.Append(Para("1.   Độ sẵn sàng", fontSize: 24));
                Bullet("Chỉ số MTTR", rec.MTTR, " phút");
                Bullet("Chỉ số MTBF", rec.MTBF, " giờ");
                Bullet("Phần trăm dừng máy", rec.StopPct, " %");
                Bullet("Phần trăm hỏng máy", rec.FaultPct, " %");
                Bullet("Availability Rate", rec.A, " %");
                //Note("Đánh giá nếu trên 90% là tốt, dưới 90% là xấu");

                // ỔN ĐỊNH 
                body.Append(Para("2.   Đánh giá độ ổn định", fontSize: 24));
                Bullet("Tổn thất tốc độ", rec.SpeedLossPct, " %");
                Bullet("Tốc độ trung bình", rec.Vtb, " gói/phút");
                Bullet("Thời gian dừng nhỏ", rec.MinorStopPct, " %");
                Bullet("Performance Rate", rec.P, " %");
                //Note("Đánh giá nếu trên 90% là tốt, dưới 90% là xấu");

                // CHẤT LƯỢNG 
                body.Append(Para("3.   Đánh giá chất lượng", fontSize: 24));
                Bullet("Phần trăm gói cấn gia vị", rec.SpicePct, " %");
                Bullet("Phần trăm gói rỗng", rec.EmptyPct, " %");
                Bullet("Quality Rate", rec.Q, " %");
                //Note("Đánh giá nếu trên 90% là tốt, dưới 90% là xấu");

                // HIỆU SUẤT 
                body.Append(Para("4.   Đánh giá tổng thể hiệu suất", fontSize: 24));
                Bullet("OEE1", rec.OEE1, " %");
                Bullet("OEE2", rec.OEE2, " %");
                Bullet("OEE3", rec.OEE3, " %");

                Blank();
                body.Append(Para("Người vận hành", fontSize: 24, just: JustificationValues.Right));
                body.Append(Para("Họ và tên", fontSize: 24, just: JustificationValues.Right));

                main.Document.Save();
            }
            // trả về file Word đã tạo 
            var fileName = $"BaoCao_PLC_Line{line}_May{machine}_Ca{caSo}_{DateTime.Now:yyyyMMdd_HHmmss}.docx";
            return File(ms.ToArray(),
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                fileName);
        }
    }
}

