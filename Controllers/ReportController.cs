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
    }
}
