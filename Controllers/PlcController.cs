using Microsoft.AspNetCore.Mvc;
using S7.Net;
using TEST1_SCADA.Models;

namespace TEST1_SCADA.Controllers
{
    [Route("plc")]
    public class PlcController : Controller
    {
        private static Plc _plc = new Plc(CpuType.S71200, "192.168.56.1", 0, 1);
        private readonly ILogger<PlcController> _logger;

        public PlcController(ILogger<PlcController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index() => View();

        private void EnsureConnected()
        {
            if (_plc == null)
                _plc = new Plc(CpuType.S71200, "192.168.56.1", 0, 1);

            if (!_plc.IsConnected)
                _plc.Open();
        }

        // ✅ POST /plc/connect
        [HttpPost("connect")]
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
        [HttpPost("WriteParameters")]
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
    }
}
