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
    /// <param name="shopifyProductIds"></param>
    /// <returns></returns>
    Task<IDictionary<string, ProductPriceModel>> GetProductsPrice(IEnumerable<string> shopifyProductIds);
}
