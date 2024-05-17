using Kentico.Xperience.Ecommerce.Common.ContentItemSynchronization;

using Shopify.ContentTypes;

namespace Kentico.Xperience.Shopify.Synchronization;


/// <summary>
/// Interface for interacting with Shopify content items.
/// </summary>
public interface IShopifyContentItemService : IContentItemService
{
    /// <summary>
    /// Retrieves Shopify product variants by their content item IDs.
    /// </summary>
    /// <param name="variantIDs">An array of variant IDs.</param>
    /// <returns>A collection of <see cref="ShopifyProductVariantItem"/>.</returns>
    Task<IEnumerable<ShopifyProductVariantItem>> GetVariants(int[] variantIDs);


    /// <summary>
    /// Retrieves Shopify product variants by their GraphQL IDs.
    /// </summary>
    /// <param name="variantGraphQLIds">An array of variant GraphQL IDs.</param>
    /// <returns>A collection of <see cref="ShopifyProductVariantItem"/>.</returns>
    Task<IEnumerable<ShopifyProductVariantItem>> GetVariants(string[] variantGraphQLIds);
}
