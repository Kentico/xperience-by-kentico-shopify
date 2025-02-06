using Kentico.Xperience.Shopify.Products.Models;

using ShopifySharp.GraphQL;

namespace Kentico.Xperience.Shopify.Products
{
    /// <summary>
    /// Service for retrieving products from Shopify store.
    /// </summary>
    public interface IShopifyProductService
    {
        /// <summary>
        /// Get products in collection
        /// </summary>
        /// <param name="initialFilter">Product filter properties.</param>
        /// <returns>Retrieved Shopify products wrapped inside <see cref="ListResultWrapper{T}"/>.</returns>
        Task<ListResultWrapper<ShopifyProductListModel>> GetProductsAsync(ProductFilter? initialFilter = null);

        /// <summary>
        /// Get product variants
        /// </summary>
        /// <param name="shopifyProductID">Shopify product ID.</param>
        /// <param name="countryCode">The country code.</param>
        /// <returns>Dictionary where key is Shopify variant ID</returns>
        Task<Dictionary<string, ProductVariantListModel>> GetProductVariants(string shopifyProductID, CountryCode countryCode);

        Task<ShopifySharp.Lists.ListResult<ShopifySharp.Product>> GetAllProductsRaw();
    }
}
