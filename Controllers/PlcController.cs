using Microsoft.AspNetCore.Mvc;
using S7.Net;
using TEST1_SCADA.Data;
using TEST1_SCADA.Models;
using Microsoft.EntityFrameworkCore; // để hỗ trợ các phương thức async của EF Core


namespace TEST1_SCADA.Controllers
{
    [Route("plc")]
    public class PlcController : Controller
    {
        private static Plc _plc = new Plc(CpuType.S71200, "192.168.56.1", 0, 1);    // PLC instance dùng chung
        private readonly ApplicationDbContext _db;          // thêm ApplicationDbContext để truy cập DB 
        private readonly ILogger<PlcController> _logger;    // thêm logger để ghi log

        public PlcController(ILogger<PlcController> logger, ApplicationDbContext db)
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

        // ✅ POST /plc/connect
        [HttpPost("connect")] // kết nối đến PLC
        public IActionResult Connect()
        {
            try
            {
                EnsureConnected();
                return Json(new { success = _plc.IsConnected, status = _plc.IsConnected ? "Kết nối đến PLC thành công!" : "Kết nối đến PLC thất bại!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi kết nối đến PLC");
                return StatusCode(500, new { success = false, status = $"Lỗi khi kết nối đến PLC: {ex.Message}" });
            }
        }

        // ✅ POST /plc/WriteParameters
        [HttpPost("WriteParameters")]   // ghi thông số xuống PLC
        public IActionResult WriteParameters([FromBody] PlcParameterWrite dto)
        {
            try
            {
                if (dto == null) return BadRequest("DTO null");

                EnsureConnected();

                // ✅ gợi ý: dùng Int16/UInt16 cho DBW
                _plc.Write("DB2.DBW0", (short)dto.TocDoChuan);
                _plc.Write("DB2.DBW2", (short)dto.ThoiGianDungMay);
                _plc.Write("DB2.DBW4", (short)dto.ChapNhanGoi_x01s);
                _plc.Write("DB2.DBW6", (short)dto.GoiCan_x01s);
                _plc.Write("DB2.DBW8", (short)dto.DayGoiCan_x01s);
                _plc.Write("DB2.DBW10", (short)dto.CapNhatTuPLC_s);

                _plc.Write("DB2.DBW12", (short)dto.CaSo);         // 1..3
                _plc.Write("DB2.DBW14", (short)dto.DayChuyen);  // 1..4
                _plc.Write("DB2.DBW16", (short)dto.May);        // 1..4
                _plc.Write("DB2.DBW18", (short)dto.SanPhamId);

                return Ok(new { success = true, message = "Đã ghi thông số xuống PLC (DB2)" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WriteParameters failed");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        // 
        [HttpPost("monitor/context")] // lưu ngữ cảnh giám sát từ UI
        // MonitorContextRequest: DTO nhận từ client 
        public async Task<IActionResult> SaveMonitorContext([FromBody] MonitorContextRequest req)
        {
            try
            {
                var tenDayChuyen = $"Dây chuyền {req.Line}";
                var tenMay = $"Máy {req.Machine}";
                var caText = $"Ca {req.CaSo}";

                // 1) lấy SanPhamId từ ParameterSettings
                var ps = await _db.ParameterSettings        // truy vấn ParameterSettings
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync(x =>
                        x.TenDayChuyen == tenDayChuyen &&
                        x.TenMay == tenMay &&
                        x.Ca == caText
                    );

                // 2) lấy TruongCaId từ ShiftConfigs
                var sc = await _db.ShiftConfigs             // truy vấn ShiftConfigs
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync(x => x.CaSo == req.CaSo);

                var record = new MonitorRecord      // tạo bản ghi MonitorRecord mới
                {
                    Line = req.Line,                // 1..4
                    Machine = req.Machine,          // 1..4
                    CaSo = req.CaSo,                // 1..3
                    PollMs = req.PollMs,            // chu kỳ đọc ms

                    SanPhamId = ps?.SanPhamId,      // lấy SanPhamId từ ParameterSettings
                    TruongCaId = sc?.TruongCaId     // lấy TruongCaId từ ShiftConfigs
                };

                _db.MonitorRecords.Add(record);
                await _db.SaveChangesAsync();

                // 3) trả về context đã resolve tên (để fill UI)
                var sanPham = record.SanPhamId != null          // lấy mã/tên sp theo SanPhamId
                    ? await _db.SanPham.FirstOrDefaultAsync(x => x.Id == record.SanPhamId)
                    : null;

                var truongCa = record.TruongCaId != null        // lấy tên trưởng ca theo TruongCaId
                    ? await _db.TruongCa.FirstOrDefaultAsync(x => x.Id == record.TruongCaId)
                    : null;
                // 4) trả về kết quả
                return Json(new
                {
                    success = true,
                    recordId = record.Id,                       // trả về Id của bản ghi mới tạo
                    productCode = sanPham?.MaSanPham ?? "",     // mã sản phẩm
                    productName = sanPham?.TenSanPham ?? "",    // tên sản phẩm
                    leaderCode = truongCa?.MaTruongCa ?? "",    // mã trưởng ca
                    leaderName = truongCa?.HovaTen ?? truongCa?.HovaTen ?? "" // tên trưởng ca 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message }); // lỗi server 
            }
        }
    }
}
