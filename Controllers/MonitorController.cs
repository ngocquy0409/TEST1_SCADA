using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TEST1_SCADA.Models;
using S7.Net;

namespace TEST1_SCADA.Controllers
{
    public class MonitorController : Controller
    {
        private static Plc _plc = new Plc(CpuType.S71200, "192.168.56.1", 0, 0); // Tạo kết nối với PLC S71200

        private readonly ILogger<MonitorController> _logger; // Biến để ghi log

        public MonitorController(ILogger<MonitorController> logger) // 
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost] // Xử lý yêu cầu POST từ biểu mẫu
        public IActionResult Connect()
        {
            _plc = new Plc(CpuType.S71200, "192.168.56.1", 0, 1); // Tạo kết nối với PLC S71200 
            try
            {
                _plc.Open(); // Kết nối đến PLC
                if (_plc.IsConnected)
                {
                    return Json(new { status = "Kết nối đến PLC thành công!" });
                }
                else
                {
                    return Json(new { status = "Kết nối đến PLC thất bại!" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi kết nối đến PLC"); // Ghi log lỗi
                return Json(new { status = $"Lỗi khi kết nối đến PLC: {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult DataWriting(TEST1_SCADA.Models.WriteData model) // Viết dữ liệu lên PLC
        {
            if (_plc != null && _plc.IsConnected)
            {
                _plc.WriteClass(model, 2, 0); // Ghi dữ liệu vào DB2, bắt đầu từ byte 0
                return Json(new { success = true, message = "Ghi dữ liệu thành công!" });
            }
            return Json(new { success = false, message = "Kết nối đến PLC không thành công" });
        }

        [HttpGet]
        public IActionResult DataReading()
        {
            try
            {
                // Đảm bảo kết nối đến PLC
                if (_plc == null)
                    _plc = new Plc(CpuType.S71200, "192.168.56.1", 0, 1);
                // Mở kết nối nếu chưa kết nối
                if (!_plc.IsConnected)
                    _plc.Open();
                // Đọc dữ liệu từ DB1, bắt đầu từ byte 0, đọc 6 byte (3 biến short)
                ushort raw1 = (ushort)_plc.Read("DB1.DBW0");
                ushort raw2 = (ushort)_plc.Read("DB1.DBW2");
                ushort raw3 = (ushort)_plc.Read("DB1.DBW4");
                // Chuyển đổi dữ liệu đọc được sang kiểu short
                short a1 = unchecked((short)raw1);
                short a2 = unchecked((short)raw2);
                short a3 = unchecked((short)raw3);
                // Trả về dữ liệu dưới dạng JSON
                return Json(new { success = true, a1, a2, a3 });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
