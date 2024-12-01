namespace Uniqlol.ViewModels.Baskets
{
    public class BasketItemVM
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public decimal SalePrice { get; set; }
        public int Discount { get; set; }
        public int Count { get; set; }
    }
}
