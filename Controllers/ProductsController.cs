using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TEST1_SCADA.Data;
using TEST1_SCADA.Models;   // + thêm dòng này

namespace TEST1_SCADA.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.SanPham.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var sp = await _context.SanPham.FirstOrDefaultAsync(m => m.Id == id);
            if (sp == null) return NotFound();

            return View(sp);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPham sp)   // Product -> SanPham
        {
            if (ModelState.IsValid)
            {
                _context.SanPham.Add(sp);                      // Add(product) -> Add(sp)
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sp);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var sp = await _context.SanPham.FindAsync(id);
            if (sp == null) return NotFound();

            return View(sp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SanPham sp)   // Product -> SanPham
        {
            if (id != sp.Id) return NotFound();                     // ID -> Id

            if (ModelState.IsValid)
            {
                try
                {
                    _context.SanPham.Update(sp);                    // Update(product) -> Update(sp)
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.SanPham.Any(e => e.Id == sp.Id))  // ID -> Id
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sp);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var sp = await _context.SanPham.FirstOrDefaultAsync(m => m.Id == id);
            if (sp == null) return NotFound();

            return View(sp);
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sp = await _context.SanPham.FindAsync(id);
            if (sp != null)
            {
                _context.SanPham.Remove(sp);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
