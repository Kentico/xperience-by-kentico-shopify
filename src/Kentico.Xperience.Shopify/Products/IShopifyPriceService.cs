using Kentico.Xperience.Shopify.Products.Models;

namespace Kentico.Xperience.Shopify.Products;

/// <summary>
/// Service for getting product prices from Shopify store.
/// </summary>
public interface IShopifyPriceService
{
    /// <summary>
    /// Get actual product prices.
    /// </summary>
    /// <param name="shopifyProductIds">Shopify products IDs.</param>
    /// <returns>Dictionary where key is the Shopify product ID and value is its <see cref="ProductPriceModel"/>.</returns>
    Task<IDictionary<string, ProductPriceModel>> GetProductsPrice(IEnumerable<string> shopifyProductIds);
}
