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


    }

}
