namespace Kentico.Xperience.Shopify.Products.Models;

/// <summary>
/// Represents the price details of a product.
/// </summary>
public class ProductPriceModel
{
    /// <summary>
    /// The price of the product.
    /// </summary>
    public decimal Price { get; set; }


    /// <summary>
    /// The list price of the product.
    /// </summary>
    public decimal? ListPrice { get; set; }


    /// <summary>
    /// Indicates if the product has multiple variants.
    /// </summary>
    public bool HasMultipleVariants { get; set; }
}
