using Uniqlol.Models;
using Uniqlol.ViewModels.Brands;
using Uniqlol.ViewModels.Products;

namespace Uniqlol.ViewModels.Shops
{
    public class ShopVM
    {

        public IEnumerable<BrandAndProductVM> Brands {  get; set; }
        public IEnumerable<ProductListItemVM> Products { get; set; }
        public int ProductCount { get; set; }
    }
}
