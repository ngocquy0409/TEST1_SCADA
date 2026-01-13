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

        // POST /plc/connect
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

        // POST /plc/WriteParameters
        [HttpPost("WriteParameters")]   // ghi thông số xuống PLC
        public IActionResult WriteParameters([FromBody] PlcParameterWrite dto)
        {
            try
            {
                if (dto == null) return BadRequest("DTO null");

                EnsureConnected();

                // ghi dữ liệu xuống PLC cho các máy
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

                // lấy SanPhamId từ ParameterSettings
                var ps = await _db.ParameterSettings        // truy vấn ParameterSettings
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync(x =>
                        x.TenDayChuyen == tenDayChuyen &&
                        x.TenMay == tenMay &&
                        x.Ca == caText
                    );

                // lấy TruongCaId từ ShiftConfigs
                var sc = await _db.ShiftConfigs             // truy vấn ShiftConfigs
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync(x => x.CaSo == req.CaSo);

                var record = new MonitorRecord      // tạo bản ghi MonitorRecord mới
                {
                    Line = req.Line,                // 1..4
                    Machine = req.Machine,          // 1..4
                    CaSo = req.CaSo,                // 1..3
                    // PollMs = req.PollMs,            // chu kỳ đọc ms

                    SanPhamId = ps?.SanPhamId,      // lấy SanPhamId từ ParameterSettings
                    TruongCaId = sc?.TruongCaId     // lấy TruongCaId từ ShiftConfigs
                };

                _db.MonitorRecords.Add(record);
                await _db.SaveChangesAsync();
                
                // trả về context đã resolve tên (để fill UI)
                var sanPham = record.SanPhamId != null          // lấy mã/tên sp theo SanPhamId
                    ? await _db.SanPham.FirstOrDefaultAsync(x => x.Id == record.SanPhamId)
                    : null;

                var truongCa = record.TruongCaId != null        // lấy tên trưởng ca theo TruongCaId
                    ? await _db.TruongCa.FirstOrDefaultAsync(x => x.Id == record.TruongCaId)
                    : null;
                var pollMs = Math.Max(200, (ps?.ThoiGianCapNhatTuPLC ?? 1) * 1000);   // tính PollMs từ tham số người dùng (tối thiểu 200ms)
                // trả về kết quả
                return Json(new
                {
                    success = true,
                    recordId = record.Id,                       // trả về Id của bản ghi mới tạo
                    productCode = sanPham?.MaSanPham ?? "",     // mã sản phẩm
                    productName = sanPham?.TenSanPham ?? "",    // tên sản phẩm
                    leaderCode = truongCa?.MaTruongCa ?? "",    // mã trưởng ca
                    leaderName = truongCa?.HovaTen ?? truongCa?.HovaTen ?? "", // tên trưởng ca 
                    pollMs = pollMs
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message }); // lỗi server 
            }
        }
        // đọc dữ liệu từ DB PLC 
        private int ReadDbInt(int db, int dbwOffset)
        {
            var obj = _plc.Read($"DB{db}.DBW{dbwOffset}");  // đọc dữ liệu từ DB PLC 
            // có thể trả short hoặc ushort tùy
            if (obj is short s) return s;       // nếu là short thì trả về short
            if (obj is ushort us) return us;    // nếu là ushort thì trả về ushort
            return Convert.ToInt32(obj);        // nếu không phải short/ushort thì chuyển đổi sang int
        }

        // hàm đọc dữ liệu của 1 máy từ DB PLC
        private (int Speed, int Oee, int Stop5s, int Stop5m, int EmptyPct, int Total, int Good, int Jam, int Empty) ReadOneMachineDb(int db)
        {
            // A1..A9: DBW0..16 (mỗi int 2 byte)
            var speed = ReadDbInt(db, 0);
            var oee = ReadDbInt(db, 2);
            var stop5s = ReadDbInt(db, 4);
            var stop5m = ReadDbInt(db, 6);
            var emptyPct = ReadDbInt(db, 8);
            var total = ReadDbInt(db, 10);
            var good = ReadDbInt(db, 12);
            var jam = ReadDbInt(db, 14);
            var empty = ReadDbInt(db, 16);

            return (speed, oee, stop5s, stop5m, emptyPct, total, good, jam, empty);
        }
        // hàm đọc và lưu dữ liệu giám sát từ PLC
        [HttpGet("read-save")]
        public async Task<IActionResult> ReadSave([FromQuery] int line = 1, [FromQuery] int machine = 1, [FromQuery] int caSo = 1)
        {
            try
            {
                EnsureConnected();

                //  tạo các biến đọc dữ liệu từ PLC cho 4 máy tương ứng DB3 DB4 DB5 DB6 ứng với máy 1 2 3 4
                var m1 = ReadOneMachineDb(3);
                var m2 = ReadOneMachineDb(4);
                var m3 = ReadOneMachineDb(5);
                var m4 = ReadOneMachineDb(6);

                // Sản phẩm: ParameterSettings theo Dây chuyền/Máy/Ca
                var tenDayChuyen = $"Dây chuyền {line}";
                var tenMay = $"Máy {machine}";
                var caText = $"Ca {caSo}";

                var ps = await _db.ParameterSettings
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync(x => x.TenDayChuyen == tenDayChuyen && x.TenMay == tenMay && x.Ca == caText);

                var sp = ps?.SanPhamId != null
                    ? await _db.SanPham.FirstOrDefaultAsync(x => x.Id == ps.SanPhamId)
                    : null;

                // Trưởng ca: ShiftConfigs theo CaSo -> TruongCaId
                var sc = await _db.ShiftConfigs
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync(x => x.CaSo == caSo);

                var tc = sc != null
                    ? await _db.TruongCa.FirstOrDefaultAsync(x => x.Id == sc.TruongCaId)
                    : null;

                // trả DTO cho UI 
                var dto = new MonitorLiveDto        // tao DTO trả về UI 
                {
                    ok = true,
                    updatedAt = DateTime.Now.ToString("HH:mm:ss"),

                    maTruongCa = tc?.MaTruongCa ?? "",
                    tenTruongCa = tc?.HovaTen ?? "",

                    maSanPham = sp?.MaSanPham ?? ps?.MaSanPham ?? "",
                    tenSanPham = sp?.TenSanPham ?? ps?.TenSanPham ?? "",

                    M1_Speed = m1.Speed,
                    M1_Oee = m1.Oee,
                    M1_Stop5s = m1.Stop5s,
                    M1_Stop5m = m1.Stop5m,
                    M1_EmptyPct = m1.EmptyPct,
                    M1_Total = m1.Total,
                    M1_Good = m1.Good,
                    M1_Jam = m1.Jam,
                    M1_Empty = m1.Empty,

                    M2_Speed = m2.Speed,
                    M2_Oee = m2.Oee,
                    M2_Stop5s = m2.Stop5s,
                    M2_Stop5m = m2.Stop5m,
                    M2_EmptyPct = m2.EmptyPct,
                    M2_Total = m2.Total,
                    M2_Good = m2.Good,
                    M2_Jam = m2.Jam,
                    M2_Empty = m2.Empty,

                    M3_Speed = m3.Speed,
                    M3_Oee = m3.Oee,
                    M3_Stop5s = m3.Stop5s,
                    M3_Stop5m = m3.Stop5m,
                    M3_EmptyPct = m3.EmptyPct,
                    M3_Total = m3.Total,
                    M3_Good = m3.Good,
                    M3_Jam = m3.Jam,
                    M3_Empty = m3.Empty,

                    M4_Speed = m4.Speed,
                    M4_Oee = m4.Oee,
                    M4_Stop5s = m4.Stop5s,
                    M4_Stop5m = m4.Stop5m,
                    M4_EmptyPct = m4.EmptyPct,
                    M4_Total = m4.Total,
                    M4_Good = m4.Good,
                    M4_Jam = m4.Jam,
                    M4_Empty = m4.Empty,
                };
                // tạo biến 
                var pollMs = Math.Max(200, (ps?.ThoiGianCapNhatTuPLC ?? 1) * 1000);
                var rec = new MonitorRecord
                {
                    Line = line,
                    Machine = machine,
                    CaSo = caSo,
                    PollMs = pollMs,
                    CreatedAt = DateTime.Now,
                    TruongCaId = sc?.TruongCaId,
                    SanPhamId = ps?.SanPhamId,

                    M1_Speed = dto.M1_Speed,
                    M1_Oee = dto.M1_Oee,
                    M1_Stop5s = dto.M1_Stop5s,
                    M1_Stop5m = dto.M1_Stop5m,
                    M1_EmptyPct = dto.M1_EmptyPct,
                    M1_Total = dto.M1_Total,
                    M1_Good = dto.M1_Good,
                    M1_Jam = dto.M1_Jam,
                    M1_Empty = dto.M1_Empty,

                    M2_Speed = dto.M2_Speed,
                    M2_Oee = dto.M2_Oee,
                    M2_Stop5s = dto.M2_Stop5s,
                    M2_Stop5m = dto.M2_Stop5m,
                    M2_EmptyPct = dto.M2_EmptyPct,
                    M2_Total = dto.M2_Total,
                    M2_Good = dto.M2_Good,
                    M2_Jam = dto.M2_Jam,
                    M2_Empty = dto.M2_Empty,

                    M3_Speed = dto.M3_Speed,
                    M3_Oee = dto.M3_Oee,
                    M3_Stop5s = dto.M3_Stop5s,
                    M3_Stop5m = dto.M3_Stop5m,
                    M3_EmptyPct = dto.M3_EmptyPct,
                    M3_Total = dto.M3_Total,
                    M3_Good = dto.M3_Good,
                    M3_Jam = dto.M3_Jam,
                    M3_Empty = dto.M3_Empty,

                    M4_Speed = dto.M4_Speed,
                    M4_Oee = dto.M4_Oee,
                    M4_Stop5s = dto.M4_Stop5s,
                    M4_Stop5m = dto.M4_Stop5m,
                    M4_EmptyPct = dto.M4_EmptyPct,
                    M4_Total = dto.M4_Total,
                    M4_Good = dto.M4_Good,
                    M4_Jam = dto.M4_Jam,
                    M4_Empty = dto.M4_Empty,
                };

                _db.MonitorRecords.Add(rec);
                await _db.SaveChangesAsync();

                return Json(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "read-save failed");
                return Json(new { ok = false, message = ex.Message });
            }
        }
        // hàm đọc 1 byte từ DB PLC 
        private byte ReadDbByte(int db, int dbbOffset)
        {
            var obj = _plc.Read($"DB{db}.DBB{dbbOffset}");
            return Convert.ToByte(obj);
        }

        private string DecodeMachineStatus(byte b)
        {
            bool stop = (b & 0b0000_0001) != 0; // bit0
            bool run = (b & 0b0000_0010) != 0; // bit1

            if (run) return "RUN";
            if (stop) return "STOP";
            return "UNKNOWN";
        }
        // GET /plc/machine-status
        [HttpGet("machine-status")]
        public IActionResult MachineStatus([FromQuery] int line = 1)
        {
            try
            {
                EnsureConnected(); // nếu bạn muốn bắt buộc connect trước thì đổi như Bước 5

                // DB12: 4 byte trạng thái
                var s1 = ReadDbByte(12, 0);
                var s2 = ReadDbByte(12, 1);
                var s3 = ReadDbByte(12, 2);
                var s4 = ReadDbByte(12, 3);

                return Json(new
                {
                    ok = true,
                    updatedAt = DateTime.Now.ToString("HH:mm:ss"),
                    line = line,
                    m1 = DecodeMachineStatus(s1),
                    m2 = DecodeMachineStatus(s2),
                    m3 = DecodeMachineStatus(s3),
                    m4 = DecodeMachineStatus(s4)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MachineStatus failed");
                return Json(new { ok = false, message = ex.Message });
            }
        }

    }
}