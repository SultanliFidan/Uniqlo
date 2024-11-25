using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uniqlol.DataAccess;
using Uniqlol.Extensions;
using Uniqlol.Models;
using Uniqlol.ViewModels.Products;

namespace Uniqlol.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController(IWebHostEnvironment _env,UniqloDbContext _context) : Controller
    {

        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.Include(x => x.Brand).ToListAsync());
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Brands.Where(x => !x.IsDeleted).ToListAsync();
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateVM vm)
        {
            if (vm.File != null)
            {
                if (!vm.File.IsValidType("image"))
                    ModelState.AddModelError("File", "File must be an image");
                if (!vm.File.IsValidSize(400))
                    ModelState.AddModelError("File", "File must be less than 400kb");
            }
            if (vm.OtherFiles.Any())
            {
                if (!vm.OtherFiles.All(x => x.IsValidType("image")))
                {
                    string fileNames = string.Join(',', vm.OtherFiles.Where(x => !x.IsValidType("image")).Select(x => x.FileName));
                    ModelState.AddModelError("OtherFiles", fileNames + " is (are) not an image");
                }
                if (!vm.OtherFiles.All(x => x.IsValidSize(400)))
                {
                    string fileNames = string.Join(',', vm.OtherFiles.Where(x => !x.IsValidSize(400)).Select(x => x.FileName));
                    ModelState.AddModelError("OtherFiles", fileNames + " is (are) bigger than 400kb");
                }
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Brands.Where(x => !x.IsDeleted).ToListAsync();
                return View(vm);
            }
            if (!await _context.Brands.AnyAsync(x => x.Id == vm.BrandId))
            {
                ViewBag.Categories = await _context.Brands.Where(x => !x.IsDeleted).ToListAsync();
                ModelState.AddModelError("BrandId", "Brand not found");
                return View();

            }
            
            Product product = vm;
            product.CoverImage = await vm.File!.UploadAsync(_env.WebRootPath, "imgs", "products");
             product.Images = vm.OtherFiles.Select(x => new ProductImage
            {
                ImageUrl = x.UploadAsync(_env.WebRootPath, "imgs", "products").Result 
            }).ToList();
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null)
            {
                return NotFound();
            }
            var path = Path.Combine("wwwroot", "imgs", "products",product.CoverImage);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            var data = await _context.Products.FindAsync(id);
            if (data is null) return NotFound();
            return View(data);


        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, ProductCreateVM vm)
        {
            if (vm.File != null)
            {
                if (!vm.File.IsValidType("image"))
                    ModelState.AddModelError("File", "File must be an image");
                if (!vm.File.IsValidSize(400))
                    ModelState.AddModelError("File", "File must be less than 400kb");
            }
            if (vm.OtherFiles.Any())
            {
                if (!vm.OtherFiles.All(x => x.IsValidType("image")))
                {
                    string fileNames = string.Join(',', vm.OtherFiles.Where(x => !x.IsValidType("image")).Select(x => x.FileName));
                    ModelState.AddModelError("OtherFiles", fileNames + " is (are) not an image");
                }
                if (!vm.OtherFiles.All(x => x.IsValidSize(400)))
                {
                    string fileNames = string.Join(',', vm.OtherFiles.Where(x => !x.IsValidSize(400)).Select(x => x.FileName));
                    ModelState.AddModelError("OtherFiles", fileNames + " is (are) bigger than 400kb");
                }
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Brands.Where(x => !x.IsDeleted).ToListAsync();
                return View(vm);
            }
            if (!await _context.Brands.AnyAsync(x => x.Id == vm.BrandId))
            {
                ViewBag.Categories = await _context.Brands.Where(x => !x.IsDeleted).ToListAsync();
                ModelState.AddModelError("BrandId", "Brand not found");
                return View();
            }

            var entity = await _context.Products.FindAsync(id);
            if (entity is null) return NotFound();
            entity.Name = vm.Name;
            entity.Description = vm.Description;
            entity.Quantity = vm.Quantity;
            entity.Discount = vm.Discount;
            entity.BrandId = vm.BrandId;
            entity.SalePrice = vm.SalePrice;
            entity.CostPrice = vm.CostPrice;
            
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }

    }
}