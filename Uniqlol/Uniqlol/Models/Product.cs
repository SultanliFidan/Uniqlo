using System.ComponentModel.DataAnnotations;

namespace Uniqlol.Models;

public class Product : BaseEntity
{
    [MaxLength(32)]
    public string Name { get; set; }
    [MaxLength(64)]
    public string Description { get; set; }
    public string CoverImage { get; set; }
    [Range(0,int.MaxValue)]
    public int Quantity { get; set; }
    [DataType("decimal(18,2)")]
    public decimal CostPrice { get; set; }
    [DataType("decimal(18,2)")]
    public decimal SalePrice { get; set; }
    [Range(0,100)]
    public int Discount { get; set; }
    public int? BrandId { get; set; }
    public Brand? Brand { get; set; }
}
