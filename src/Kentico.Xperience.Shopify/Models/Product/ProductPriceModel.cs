﻿namespace Kentico.Xperience.Shopify.Models;

public class ProductPriceModel
{
    public decimal Price { get; set; }
    public decimal? ListPrice { get; set; }
    public bool HasMultipleVariants { get; set; }
}
