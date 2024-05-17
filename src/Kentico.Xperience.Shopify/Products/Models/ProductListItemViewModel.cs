namespace Kentico.Xperience.Shopify.Products.Models;

/// <summary>
/// View model for a product list item.
/// </summary>
public class ProductListItemViewModel
{
    /// <summary>
    /// Price detail of the product.
    /// </summary>
    public required ProductCatalogPrices PriceDetail { get; set; }

    /// <summary>
    /// Name of the product.
    /// </summary>
    public required string Name { get; set; }


    /// <summary>
    /// Image path of the product.
    /// </summary>
    public required string ImagePath { get; set; }


    /// <summary>
    /// Public status name of the product.
    /// </summary>
    public required string PublicStatusName { get; set; }


    /// <summary>
    /// Indicates if the product is available.
    /// </summary>
    public bool Available { get; set; }


    /// <summary>
    /// URL of the product.
    /// </summary>
    public required string Url { get; set; }


    /// <summary>
    /// Short description of the product.
    /// </summary>
    public required string ShortDescription { get; set; }
}


/// <summary>
/// Represents the prices of a product in the product catalog.
/// </summary>
public class ProductCatalogPrices
{
    /// <summary>
    /// List price of the product.
    /// </summary>
    public decimal ListPrice { get; set; }


    /// <summary>
    /// Price of the product.
    /// </summary>
    public decimal Price { get; set; }
}
