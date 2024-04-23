using CMS.ContentEngine;
using Kentico.Xperience.Ecommerce.Common.ContentItemSynchronization;
using Shopify.ContentTypes;

namespace Kentico.Xperience.Shopify.ProductSynchronization;
public class ShopifyContentItemService : ContentItemServiceBase, IShopifyContentItemService
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
            global::Shopify.ProductVariant.CONTENT_TYPE_NAME,
            q => q.Where(w => w.WhereIn(nameof(ShopifyProductVariantItem.SystemFields.ContentItemID), variantIDs)));
    }
}
