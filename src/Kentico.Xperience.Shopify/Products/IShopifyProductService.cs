using Kentico.Xperience.Shopify.Products.Models;

using ShopifySharp;
using ShopifySharp.Filters;
using ShopifySharp.Lists;

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
        Task<ListResultWrapper<ShopifyProductListModel>> GetProductsAsync(ProductFilter initialFilter);

        /// <summary>
        /// Get filtered products. This method is used for paging options.
        /// </summary>
        /// <param name="filterParams">Filter with Limit parameter and PageInfo returned in header of last API response</param>
        /// <returns>Retrieved Shopify products wrapped inside <see cref="ListResultWrapper{T}"/>.</returns>
        Task<ListResultWrapper<ShopifyProductListModel>> GetProductsAsync(PagingFilterParams filterParams);

        /// <summary>
        /// Get all products with variants without presetment prices,
        /// </summary>
        /// <param name="filter">Products list filter.</param>
        /// <returns>Retrieved Shopify products wrapped inside <see cref="ListResult{T}"/>.</returns>
        Task<ListResult<Product>> GetAllProductsRaw(ListFilter<Product> filter);

        /// <summary>
        /// Get all products with variants without presetment prices.
        /// </summary>
        /// <returns>Unmodified result retrieved from <see cref="IProductService.ListAsync(ProductListFilter, bool, CancellationToken)"/></returns>
        Task<ListResult<Product>> GetAllProductsRaw();

        /// <summary>
        /// Get product variants
        /// </summary>
        /// <param name="shopifyProductID">Shopify product ID.</param>
        /// <param name="currencyCode">The currency code (ISO 4217).</param>
        /// <returns>Dictionary where key is Shopify variant ID</returns>
        Task<Dictionary<string, ProductVariantListModel>> GetProductVariants(string shopifyProductID, string currencyCode);
    }
}
