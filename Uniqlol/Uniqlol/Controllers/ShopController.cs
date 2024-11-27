using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uniqlol.DataAccess;
using Uniqlol.ViewModels.Brands;
using Uniqlol.ViewModels.Products;
using Uniqlol.ViewModels.Shops;

namespace Uniqlol.Controllers
{
    public class ShopController(UniqloDbContext _context) : Controller
    {
        public async Task<IActionResult> Index(int? catId, string amount)
        {
            var query = _context.Products.AsQueryable();
            if (catId.HasValue)
            {
                query = query.Where(x => x.BrandId == catId);
            }
            if (amount != null)
            {
               var prices = amount.Split('-').Select(x => Convert.ToInt32(x));
                query = query.Where(y => prices.ElementAt(0) <= y.SalePrice && prices.ElementAt(1) >= y.SalePrice);
            }
            ShopVM vm = new ShopVM();
            vm.Brands = await _context.Brands.Where(x => !x.IsDeleted).Select(x => new BrandAndProductVM
            {
                Id = x.Id,
                Name = x.Name,
                Count = x.Products.Count
            }).ToListAsync();

            vm.Products = await query.Take(4).Select(x => new ProductListItemVM
            {
                CoverImage = x.CoverImage,
                Discount = x.Discount,
                Id = x.Id,
                IsInStock = x.Quantity > 0,
                Name = x.Name,
                SalePrice = x.SalePrice
            }).ToListAsync();
            vm.ProductCount = query.Count();
            return View(vm);
        }
    }
}
