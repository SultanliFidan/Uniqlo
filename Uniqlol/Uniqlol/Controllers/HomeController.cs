using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Uniqlol.DataAccess;
using Uniqlol.ViewModels.Commons;
using Uniqlol.ViewModels.Products;
using Uniqlol.ViewModels.Sliders;


namespace Uniqlol.Controllers
{
    public class HomeController(UniqloDbContext _context) : Controller
    {

        public async Task<IActionResult> Index()
        {
            HomeVM vm = new();
            vm.Sliders = await _context.Sliders.Select(x => new SliderListItemVM
            {
                ImageUrl = x.ImageUrl,
                Link = x.Link,
                Subtitle = x.Subtitle,
                Title = x.Title
            }).ToListAsync();

            vm.Products = await _context.Products.Select(x => new ProductListItemVM
            {
                CoverImage = x.CoverImage,
                Discount = x.Discount,
                Id = x.Id,
                IsInStock = x.Quantity > 0,
                Name = x.Name,
                SalePrice = x.SalePrice
            }).ToListAsync();
            return View(vm);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }


    }
}
