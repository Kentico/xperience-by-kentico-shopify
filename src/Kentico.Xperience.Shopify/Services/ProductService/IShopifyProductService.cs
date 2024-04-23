﻿using Kentico.Xperience.Shopify.API.Models;
using Kentico.Xperience.Shopify.Models;
using ShopifySharp;
using ShopifySharp.Filters;
using ShopifySharp.Lists;

namespace Kentico.Xperience.Shopify.Services.ProductService
{
    public interface IShopifyProductService
    {
        /// <summary>
        /// Get products in collection
        /// </summary>
        /// <param name="initialFilter"></param>
        /// <returns></returns>
        Task<ListResultWrapper<ShopifyProductListModel>> GetProductsAsync(ProductFilter initialFilter);

        /// <summary>
        /// Get filtered products. This method is used for paging options.
        /// </summary>
        /// <param name="filterParams">Filter with Limit parameter and PageInfo returned in header of last API response</param>
        /// <returns></returns>
        Task<ListResultWrapper<ShopifyProductListModel>> GetProductsAsync(PagingFilterParams filterParams);

        /// <summary>
        /// Get all products with variants without presetment prices,
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ListResult<Product>> GetAllProductsRaw(ListFilter<Product> filter);

        /// <summary>
        /// Get all products with variants without presetment prices.
        /// </summary>
        /// <returns></returns>
        Task<ListResult<Product>> GetAllProductsRaw();

        /// <summary>
        /// Get product variants
        /// </summary>
        /// <param name="shopifyProductID"></param>
        /// <param name="currencyCode"></param>
        /// <returns>Dictionary where key is Shopify variant ID</returns>
        Task<Dictionary<string, ProductVariantListModel>> GetProductVariants(string shopifyProductID, string currencyCode);
    }
}
