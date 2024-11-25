﻿using Uniqlol.ViewModels.Products;
using Uniqlol.ViewModels.Sliders;

namespace Uniqlol.ViewModels.Commons
{
    public class HomeVM
    {
        public IEnumerable<SliderListItemVM> Sliders { get; set; }
        public IEnumerable<ProductListItemVM> Products { get; set; }
    }
}
