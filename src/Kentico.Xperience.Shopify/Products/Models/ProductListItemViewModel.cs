namespace Kentico.Xperience.Shopify.Products.Models;
public class ProductListItemViewModel
{
    public required ProductCatalogPrices PriceDetail { get; set; }
    public required string Name { get; set; }
    public required string ImagePath { get; set; }
    public required string PublicStatusName { get; set; }
    public bool Available { get; set; }
    public required string Url { get; set; }
    public required string ShortDescription { get; set; }
}

public class ProductCatalogPrices
{
    public decimal ListPrice { get; set; }
    public decimal Price { get; set; }
}