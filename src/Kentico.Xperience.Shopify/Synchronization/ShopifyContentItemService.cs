using CMS.ContentEngine;

using Kentico.Xperience.Ecommerce.Common.ContentItemSynchronization;

using Shopify;
using Shopify.ContentTypes;

namespace Kentico.Xperience.Shopify.Synchronization;
internal class ShopifyContentItemService : ContentItemServiceBase, IShopifyContentItemService
{
    public ShopifyContentItemService(
        IContentItemManagerFactory contentItemManagerFactory,
        IContentQueryExecutor contentQueryExecutor,
        IContentQueryResultMapper contentQueryResultMapper) : base(contentItemManagerFactory, contentQueryExecutor, contentQueryResultMapper)
    {
    }

    public async Task<IEnumerable<ShopifyProductVariantItem>> GetVariants(int[] variantIDs)
    {
        return await GetContentItems<ShopifyProductVariantItem>(
            ProductVariant.CONTENT_TYPE_NAME,
            q => q.Where(w => w.WhereIn(nameof(ShopifyProductVariantItem.SystemFields.ContentItemID), variantIDs)));
    }

    public async Task<IEnumerable<ShopifyProductVariantItem>> GetVariants(string[] variantGraphQLIds)
    {
        return await GetContentItems<ShopifyProductVariantItem>(
            ProductVariant.CONTENT_TYPE_NAME,
            q => q.Where(w => w.WhereIn(nameof(ShopifyProductVariantItem.ShopifyMerchandiseID), variantGraphQLIds)));
    }
}
