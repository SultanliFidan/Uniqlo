using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uniqlol.DataAccess;
using Uniqlol.Helpers;
using Uniqlol.ViewModels.Baskets;

namespace Uniqlol.ViewComponents
{
    public class LayoutHeaderViewComponent(UniqloDbContext _context) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var basket = BasketHelper.GetBasket(HttpContext.Request);
            var basketItems =  await _context.Products.Where(x => basket.Select(y => y.Id).Contains(x.Id))
                .Select(x => new BasketItemVM
                {
                    Id = x.Id, 
                    Name = x.Name,
                    ImageUrl = x.CoverImage,
                    SalePrice = x.SalePrice,
                    Discount = x.Discount
                }).ToListAsync();
            foreach (var item in basketItems)
            {
                item.Count = basket.First(x => x.Id == item.Id).Count;
            }
            return View(basketItems);
        }
    }
}
