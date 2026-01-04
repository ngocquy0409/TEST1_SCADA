using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TEST1_SCADA.Data;
using TEST1_SCADA.Models;
using System.Linq; // để sử dụng LINQ trong truy vấn dữ liệu
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
        
        public class SaveProductionInputDto // DTO để nhận dữ liệu từ client
        {
            public DateTime NgaySanXuat { get; set; }
            public int SanPhamId { get; set; }
            public int CaSo { get; set; }
            public string DayChuyen { get; set; } = "";
            public int SanLuongThuc { get; set; }
        }
        // API lưu dữ liệu nhập sản xuất
        [HttpPost]
        public IActionResult SaveProductionInput([FromBody] SaveProductionInputDto? dto)
        {
            if (dto == null) return BadRequest("DTO null - sai JSON");
            if (dto.SanPhamId <= 0) return BadRequest("Chưa chọn sản phẩm");
            if (dto.CaSo < 1 || dto.CaSo > 3) return BadRequest("Ca không hợp lệ");
            if (string.IsNullOrWhiteSpace(dto.DayChuyen)) return BadRequest("Chưa chọn dây chuyền");

            var existsSp = _context.SanPham.Any(x => x.Id == dto.SanPhamId);
            if (!existsSp) return BadRequest("SanPhamId không tồn tại trong DB");

            var row = new ProductionInput
            {
                NgaySanXuat = dto.NgaySanXuat.Date,
                SanPhamId = dto.SanPhamId,
                CaSo = dto.CaSo,
                DayChuyen = dto.DayChuyen,
                SanLuongThuc = dto.SanLuongThuc,
                CreatedAt = DateTime.Now
            };

            _context.ProductionInputs.Add(row);
            _context.SaveChanges();
            return Ok(new { ok = true, id = row.Id });
        }
        // kết thúc API lưu dữ liệu nhập sản xuất
        // API lấy danh sách dữ liệu nhập sản xuất để hiển thị trong bảng
        [HttpGet]
        public IActionResult GetProductionInputs()
        {
            var list = (from p in _context.ProductionInputs
                        join sp in _context.SanPham on p.SanPhamId equals sp.Id
                        orderby p.Id descending
                        select new
                        {
                            id = p.Id,
                            ngaySanXuat = p.NgaySanXuat,
                            maSanPham = sp.MaSanPham,
                            tenSanPham = sp.TenSanPham,
                            caSo = p.CaSo,
                            dayChuyen = p.DayChuyen,
                            sanLuongThuc = p.SanLuongThuc
                        })
                        .Take(200)
                        .ToList();

            return Json(list);
        }


    }

}
