using Kentico.Xperience.Shopify.Models;

namespace Kentico.Xperience.Shopify.Services;

public interface IShopifyPriceService
{
    Task<IDictionary<string, ProductPriceModel>> GetProductsPrice(IEnumerable<string> shopifyProductIds);
}
