using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
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
            vm.Sliders = await _context.Sliders.Where(x => !x.IsDeleted).Select(x => new SliderListItemVM
            {
                ImageUrl = x.ImageUrl,
                Link = x.Link,
                Subtitle = x.Subtitle,
                Title = x.Title
            }).ToListAsync();
            vm.Brands = await _context.Brands.OrderByDescending(x => x.Products!.Count).Take(4).ToListAsync();
            
            vm.PopularProducts = await _context.Products
                .Where(x => vm.Brands.Select(y => y.Id).Contains(x.BrandId.Value))
                .Select(x => new ProductListItemVM
            {
                CoverImage = x.CoverImage,
                Discount = x.Discount,
                Id = x.Id,
                IsInStock = x.Quantity > 0,
                Name = x.Name,
                SalePrice = x.SalePrice,
                BrandId = x.BrandId!.Value
            }).ToListAsync();
            return View(vm);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public void SetSession(string key, string value)
        {
            HttpContext.Session.SetString(key, value);
            HttpContext.Session.Remove(key);
        }

        public IActionResult GetSession(string key)
        {
            return Content(HttpContext.Session.GetString(key) ?? string.Empty);
        }
        public void SetCookies(string key,string value)
        {
            var opt = new CookieOptions
            {
                //Expires = new DateTime(2024,11,30),
                //Expires = DateTime.UtcNow.AddSeconds(30)
                MaxAge = TimeSpan.FromMinutes(2)
            };
            HttpContext.Response.Cookies.Append(key, value);
        }
        public IActionResult GetCookies(string key,string value)
        {
            return Content(HttpContext.Request.Cookies[key]);
        }
        public IActionResult RemoveCookies(string key)
        {
            HttpContext.Response.Cookies.Delete(key);
            return Ok();
        }
    }
}
