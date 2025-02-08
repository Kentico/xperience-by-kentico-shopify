using CMS.ContentEngine;
using CMS.Core;

using Kentico.Xperience.Ecommerce.Common.ContentItemSynchronization;
using Kentico.Xperience.Shopify.Synchronization.BulkOperations;

using Shopify.ContentTypes;

using ShopifySharp;

namespace Kentico.Xperience.Shopify.Synchronization.Variants;
internal class VariantSynchronizationService : SynchronizationServiceBase, IVariantSynchronizationService
{
    public VariantSynchronizationService(IShopifyContentItemService contentItemService, IEventLogService eventLogService)
        : base(contentItemService, eventLogService)
    {
    }


    public async Task<IEnumerable<Guid>> ProcessVariants(
        IEnumerable<ShopifyProductVariantDto> variants,
        IEnumerable<ShopifyProductVariantItem>? existingVariants,
        Dictionary<string, Guid> variantImages,
        string languageName,
        int userID)
    {
        (var toCreate, var toUpdate, var toDelete) = ClassifyItems(variants, existingVariants ?? []);

        await contentItemService.DeleteContentItems(toDelete.Select(x => x.ContentItemIdentifier), languageName, userID);

        // TODO
        var addedVariantsID = new List<int>();//await CreateProductVariants(toCreate, variantImages, languageName, userID).ToListAsync();

        // TODO
        await UpdateProductVariants([]/*toUpdate*/, variantImages, languageName, userID);

        IEnumerable<ShopifyProductVariantItem> variantsToReturn;
        if (addedVariantsID.Count != 0)
        {
            var newVariantsCI = await contentItemService.GetContentItems<ShopifyProductVariantItem>(global::Shopify.ProductVariant.CONTENT_TYPE_NAME, config =>
                config.Where(x => x.WhereIn(nameof(ShopifyProductVariantItem.SystemFields.ContentItemID), addedVariantsID))
                    .Columns(nameof(ShopifyProductVariantItem.SystemFields.ContentItemGUID), nameof(ShopifyProductVariantItem.ShopifyVariantID)));

            variantsToReturn = existingVariants == null ? newVariantsCI : newVariantsCI.Concat(existingVariants);
        }
        else
        {
            variantsToReturn = existingVariants ?? Enumerable.Empty<ShopifyProductVariantItem>();
        }

        return OrderItemsByShopify(variantsToReturn, variants);
    }


    private async Task UpdateProductVariants(
        IEnumerable<(ProductVariant ShopifyItem, ShopifyProductVariantItem ContentItem)> productVariants,
        IDictionary<string, Guid> variantsImages,
        string languageName,
        int userID)
    {
        foreach ((var shopifyVariant, var contentItemVariant) in productVariants)
        {
            var variantSyncItem = CreateVariantSynchronizationItem(variantsImages, shopifyVariant);
            if (variantSyncItem.GetModifiedProperties(contentItemVariant, out var modifiedProps))
            {
                bool updateResult = await contentItemService.UpdateContentItem(new ContentItemUpdateParams()
                {
                    ContentItemParams = modifiedProps,
                    ContentItemID = contentItemVariant.SystemFields.ContentItemID,
                    LanguageName = languageName,
                    UserID = userID,
                    VersionStatus = contentItemVariant.SystemFields.ContentItemCommonDataVersionStatus
                });

                if (!updateResult)
                {
                    LogEvent(EventTypeEnum.Error, nameof(ProcessVariants), $"Could not update item {variantSyncItem.DisplayName}");
                }
            }
        }
    }


    private async IAsyncEnumerable<int> CreateProductVariants(
        IEnumerable<ProductVariant> productVariants,
        IDictionary<string, Guid> variantsImages,
        string languageName,
        int userID)
    {
        foreach (var productVariant in productVariants)
        {
            var variantSyncItem = CreateVariantSynchronizationItem(variantsImages, productVariant);
            int itemId = await contentItemService.AddContentItem(new ContentItemAddParams()
            {
                ContentItem = variantSyncItem,
                LanguageName = languageName,
                UserID = userID
            });

            if (itemId == 0)
            {
                LogEvent(EventTypeEnum.Error, nameof(ProcessVariants), $"Could not add content item {variantSyncItem.DisplayName}");
            }
            else
            {
                yield return itemId;
            }
        }
    }


    private VariantSynchronizationItem CreateVariantSynchronizationItem(IDictionary<string, Guid> variantsImages, ProductVariant variant)
    {
        bool hasImage = variantsImages.TryGetValue(variant.Id?.ToString() ?? string.Empty, out var variantImageGuid);

        return new VariantSynchronizationItem()
        {
            ShopifyVariantID = variant.Id?.ToString() ?? string.Empty,
            Title = variant.Title,
            SKU = variant.SKU,
            Weight = variant.Weight ?? 0,
            ShopifyMerchandiseID = variant.AdminGraphQLAPIId,
            ShopifyProductID = variant.ProductId?.ToString() ?? string.Empty,
            Image = hasImage ? [new ContentItemReference() { Identifier = variantImageGuid }] : []
        };
    }
}
