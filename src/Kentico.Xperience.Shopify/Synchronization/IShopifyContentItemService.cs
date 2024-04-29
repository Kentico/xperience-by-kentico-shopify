using Kentico.Xperience.Ecommerce.Common.ContentItemSynchronization;

using Shopify.ContentTypes;

namespace Kentico.Xperience.Shopify.Synchronization;
internal interface IShopifyContentItemService : IContentItemService
{
    Task<IEnumerable<ShopifyProductVariantItem>> GetVariants(int[] variantIDs);
}
