using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TEST1_SCADA.Data;
using TEST1_SCADA.Models;
namespace TEST1_SCADA.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SettingsController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            ViewBag.TruongCa = _context.TruongCa.ToList();
            ViewBag.SanPham = _context.SanPham.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult LuuTruongCa([FromBody] TruongCa model)
        {
            _context.TruongCa.Add(model);
            _context.SaveChanges();
            return Json(model);
        }

        [HttpPost]
        public IActionResult LuuSanPham([FromBody] SanPham model)
        {
            _context.SanPham.Add(model);
            _context.SaveChanges();
            return Json(model);
        }
        [HttpPost]
        public IActionResult AddTruongCa(string maTruongCa, string hoVaTen)
        {
            var tc = new TruongCa
            {
                MaTruongCa = maTruongCa,
                HovaTen = hoVaTen
            };

            _context.TruongCa.Add(tc);
            _context.SaveChanges();

            return Json(new
            {
                Id = tc.Id,
                maTruongCa = tc.MaTruongCa,
                hovaTen = tc.HovaTen
            });
        }
        [HttpPost]
        public IActionResult AddSanPham(string maSanPham, string tenSanPham)
        {
            var sp = new SanPham
            {
                MaSanPham = maSanPham,
                TenSanPham = tenSanPham
            };

            _context.SanPham.Add(sp);
            _context.SaveChanges();

            return Json(new
            {
                Id = sp.Id,
                maSanPham = sp.MaSanPham,
                tenSanPham = sp.TenSanPham
            });
        }
        [HttpPost]
        public IActionResult DeleteTruongCa(int id)
        {
            var tc = _context.TruongCa.Find(id);
            if (tc == null) return NotFound();

            _context.TruongCa.Remove(tc);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        public IActionResult DeleteSanPham(int id)
        {
            var sp = _context.SanPham.Find(id);
            if (sp == null) return NotFound();

            _context.SanPham.Remove(sp);
            _context.SaveChanges();

            return Ok();
        }

        // tạo API lưu cấu hình cho cài đặt ca sản xuất
        public class SaveShiftConfigDto
        {
            public int CaSo { get; set; }
            public int GioBatDau { get; set; }
            public int PhutBatDau { get; set; }
            public int TruongCaId { get; set; }
        }
        [HttpPost]
        // API để lưu cấu hình ca sản xuất (ShiftConfig)
        public IActionResult SaveShiftConfig([FromBody] List<SaveShiftConfigDto> configs)
        {
            if (configs == null || configs.Count == 0)
                return BadRequest("Body JSON rỗng hoặc sai format. configs == null");
            if (configs.Count == 0)
                return BadRequest("configs rỗng");
            foreach (var c in configs)
            {
                var exist = _context.ShiftConfigs.FirstOrDefault(s => s.CaSo == c.CaSo); // kiểm tra ca số đã tồn tại chưa
                if (exist == null)
                {
                    _context.ShiftConfigs.Add(new ShiftConfig // nếu chưa tồn tại thì thêm mới
                    {
                        CaSo = c.CaSo,              // gán các thuộc tính
                        GioBatDau = c.GioBatDau,    // từ đối tượng DTO nhận vào
                        PhutBatDau = c.PhutBatDau,  // đến đối tượng ShiftConfig
                        TruongCaId = c.TruongCaId   // rồi thêm vào cơ sở dữ liệu
                    });
                }
                else    //  nếu đã tồn tại thì cập nhật lại
                {
                    exist.GioBatDau = c.GioBatDau;
                    exist.PhutBatDau = c.PhutBatDau;
                    exist.TruongCaId = c.TruongCaId;
                }
            }
            _context.SaveChanges();         // lưu thay đổi vào cơ sở dữ liệu
            return Ok(new { ok = true });                  // trả về mã trạng thái 200 OK    
        }
        // kết thúc API lưu cấu hình ca sản xuất
        [HttpGet]
        // API load cấu hình để hiển thị khi vào lại trang
        public IActionResult GetShiftConfig() // lấy dữ liệu cấu hình ca sản xuất
        {
            var data = _context.ShiftConfigs    // truy vấn bảng ShiftConfigs
                .Select(x => new        // chọn các trường cần thiết để trả về
                {
                    x.CaSo,
                    x.GioBatDau,
                    x.PhutBatDau,
                    x.TruongCaId
                })
                .ToList();

            return Json(data);
        }
    }

}
