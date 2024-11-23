using System.ComponentModel.DataAnnotations;
using Uniqlol.Models;

namespace Uniqlol.ViewModels.Products
{
    public class ProductCreateVM
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CoverImage { get; set; }
        [Range(0,int.MaxValue)]
        public int Quantity { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SalePrice { get; set; }
        [Range(0,100)]
        public int Discount { get; set; }
        public int BrandId { get; set; }
        public IFormFile File { get; set; }
        public static implicit operator Product(ProductCreateVM vm)
        {
            return new Product
            {
                Name = vm.Name,
                Description = vm.Description,
                CoverImage = vm.CoverImage,
                SalePrice = vm.SalePrice,
                Discount = vm.Discount,
                BrandId = vm.BrandId,
                CostPrice = vm.CostPrice,
                Quantity = vm.Quantity,
            };
        }
    }
}
