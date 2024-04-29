using Kentico.Xperience.Ecommerce.Common.ContentItemSynchronization;

using Shopify.ContentTypes;

namespace Kentico.Xperience.Shopify.Synchronization;

public interface IShopifyContentItemService : IContentItemService
{
    Task<IEnumerable<ShopifyProductVariantItem>> GetVariants(int[] variantIDs);

    Task<IEnumerable<ShopifyProductVariantItem>> GetVariants(string[] variantGraphQLIds);
}
