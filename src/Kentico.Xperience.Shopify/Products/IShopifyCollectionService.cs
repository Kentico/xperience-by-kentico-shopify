using Kentico.Xperience.Shopify.Products.Models;

using ShopifySharp.GraphQL;

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

        /// <summary>
        /// Get products of the specific collection.
        /// </summary>
        /// <param name="collectionId">Collection ID.</param>
        /// <param name="topN">Top N results.</param>
        /// <param name="countryCode">Country code used to retrieve prices in specific currency.</param>
        /// <returns>List of products from specified collection.</returns>
        Task<IEnumerable<ShopifyProductListModel>> GetCollectionProducts(string collectionId, int topN, CountryCode countryCode);
    }
}
