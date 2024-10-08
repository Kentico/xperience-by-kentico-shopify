﻿using Kentico.Xperience.Shopify.Products.Models;

namespace Kentico.Xperience.Shopify.Products
{
    /// <summary>
    /// Interface for interacting with Shopify collections.
    /// </summary>
    public interface IShopifyCollectionService
    {
        /// <summary>
        /// Get all product collections from Shopify store.
        /// </summary>
        /// <returns>List of collections retrieved from Shopify store.</returns>
        Task<IEnumerable<CollectionListingModel>> GetCollectionListing();
    }
}
