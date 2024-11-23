using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uniqlol.DataAccess;
using Uniqlol.Models;
using Uniqlol.ViewModels.Brands;

namespace Uniqlol.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController(UniqloDbContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await _context.Brands.ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(BrandCreateVM vm)
        {
            Brand brand = new Brand
            {
                Name = vm.Name,
            };
            await _context.Brands.AddAsync(brand);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand is null)
            {
                return NotFound();
            }
            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Update(int id)
        {
            var data = await _context.Brands.FindAsync(id);
            if (data is null) return NotFound();
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> Update(BrandCreateVM vm, int id)
        {
            var entity = await _context.Brands.FindAsync(id);
            if (entity is null) return NotFound();
            entity.Name = vm.Name;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
