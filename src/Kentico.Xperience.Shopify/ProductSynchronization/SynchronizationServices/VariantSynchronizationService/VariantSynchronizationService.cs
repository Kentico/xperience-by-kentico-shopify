using CMS.ContentEngine;
using CMS.Core;
using Kentico.Xperience.Ecommerce.Common.ContentItemSynchronization;
using Shopify.ContentTypes;
using ShopifySharp;

namespace Kentico.Xperience.Shopify.ProductSynchronization;
public class VariantSynchronizationService : SynchronizationServiceBase, IVariantSynchronizationService
{
    public VariantSynchronizationService(IShopifyContentItemService contentItemService, IEventLogService eventLogService)
        : base(contentItemService, eventLogService)
    {
    }

    public async Task<IEnumerable<Guid>> ProcessVariants(
        IEnumerable<ProductVariant> variants,
        IEnumerable<ShopifyProductVariantItem>? existingVariants,
        Dictionary<string, Guid> variantImages,
        string languageName,
        int userID)
    {
        await DeleteNonExistingItems(existingVariants, variants, languageName, userID);
        var addedVariantsID = new List<int>();

        foreach (var variant in variants)
        {
            bool hasImage = variantImages.TryGetValue(variant.Id?.ToString() ?? string.Empty, out var variantImageGuid);

            var variantSyncItem = new ProductVariantSynchronizationItem()
            {
                ShopifyVariantID = variant.Id?.ToString() ?? string.Empty,
                Title = variant.Title,
                SKU = variant.SKU,
                Weight = variant.Weight ?? 0,
                ShopifyMerchandiseID = variant.AdminGraphQLAPIId,
                Image = hasImage ? [new ContentItemReference() { Identifier = variantImageGuid }] : []
            };


            var existingVariant = existingVariants?.FirstOrDefault(x => x.ShopifyVariantID == variantSyncItem.ShopifyVariantID);
            if (existingVariant == null)
            {
                int itemID = await contentItemService.AddContentItem(new ContentItemAddParams()
                {
                    ContentItem = variantSyncItem,
                    LanguageName = languageName,
                    UserID = userID
                });

                if (itemID == 0)
                {
                    LogEvent(EventTypeEnum.Error, nameof(ProcessVariants), $"Could not add content item {variantSyncItem.DisplayName}");
                }
                else
                {
                    addedVariantsID.Add(itemID);
                }
            }
            else if (variantSyncItem.GetModifiedProperties(existingVariant, out var modifiedProps))
            {
                bool updateResult = await contentItemService.UpdateContentItem(new ContentItemUpdateParams()
                {
                    ContentItemParams = modifiedProps,
                    ContentItemID = existingVariant.SystemFields.ContentItemID,
                    LanguageName = languageName,
                    UserID = userID,
                    VersionStatus = existingVariant.SystemFields.ContentItemCommonDataVersionStatus
                });

                if (!updateResult)
                {
                    LogEvent(EventTypeEnum.Error, nameof(ProcessVariants), $"Could not update item {variantSyncItem.DisplayName}");
                }
            }
        }

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

        return OrderItemsByShopify(variantsToReturn, variants.OrderBy(x => x.Position));
    }
}
