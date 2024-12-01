using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.Json;
using Uniqlol.DataAccess;
using Uniqlol.ViewModels.Baskets;
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

        public async Task<IActionResult> AddBasket(int id)
        {
            var basket = getBasket();
            var item = basket.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                item.Count++;
            }
            else
            {
                basket.Add(new BasketCookieItemVM
                {
                    Id = id,
                    Count = 1
                });
            }
                
           
            string data = JsonSerializer.Serialize(basket);
            HttpContext.Response.Cookies.Append("basket", data);
            return Ok();
        }

        public async Task<IActionResult> GetBasket(int id)
        {

            return Json(getBasket());
        }

        public async Task<IActionResult> Remove(int id)
        {
            var basket = getBasket();
            var item = basket.FirstOrDefault(x => x.Id == id);
            basket.Remove(item);
            string data = JsonSerializer.Serialize(basket);
            HttpContext.Response.Cookies.Append("basket", data);
            return RedirectToAction(nameof(Index));
        }
        List<BasketCookieItemVM> getBasket()
        {
            try
            {
                string? value = HttpContext.Request.Cookies["basket"];
                if (value is null)
                {
                    return new();
                }
                return JsonSerializer.Deserialize<List<BasketCookieItemVM>>(value) ?? new();
            }
            catch
            {
                return new();
            }
        }




    }
}
