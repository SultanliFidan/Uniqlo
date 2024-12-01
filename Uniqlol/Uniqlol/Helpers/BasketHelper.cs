using System.Text.Json;
using Uniqlol.ViewModels.Baskets;

namespace Uniqlol.Helpers
{
    public class BasketHelper
    {
        public static List<BasketCookieItemVM> GetBasket(HttpRequest request)
        {
            string? value = request.Cookies["basket"];
            if (value is null) return new();
            return JsonSerializer.Deserialize<List<BasketCookieItemVM>>(value) ?? new();
        }
    }
}
