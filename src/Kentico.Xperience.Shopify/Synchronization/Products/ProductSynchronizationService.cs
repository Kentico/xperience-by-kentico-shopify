using CMS.ContentEngine;
using CMS.Core;

using Kentico.Xperience.Ecommerce.Common.ContentItemSynchronization;

using Shopify.ContentTypes;

using ShopifySharp;

namespace Kentico.Xperience.Shopify.Synchronization.Products;
internal class ProductSynchronizationService : SynchronizationServiceBase, IProductSynchronizationService
{
    public ProductSynchronizationService(IEventLogService eventLogService, IShopifyContentItemService contentItemService)
        : base(contentItemService, eventLogService)
    {
    }

    public async Task ProcessProduct(Product product, IEnumerable<Guid> variants, IEnumerable<Guid> images, string languageName, int userID, ShopifyProductItem? existingProduct)
    {
        var productSyncItem = new ProductSynchronizationItem()
        {
            Title = product.Title,
            Description = product.BodyHtml,
            ShopifyProductID = product.Id?.ToString() ?? "",
            Variants = variants.Select(v => new ContentItemReference()
            {
                Identifier = v
            }),
            Images = images.Select(i => new ContentItemReference()
            {
                Identifier = i
            })
        };

        if (existingProduct?.SystemFields != null &&
            existingProduct.SystemFields.ContentItemID != 0 &&
            productSyncItem.GetModifiedProperties(existingProduct, out var modifiedProps))
        {
            var updateParams = new ContentItemUpdateParams()
            {
                ContentItemParams = modifiedProps,
                ContentItemID = existingProduct.SystemFields.ContentItemID,
                LanguageName = languageName,
                UserID = userID,
                VersionStatus = existingProduct.SystemFields.ContentItemCommonDataVersionStatus
            };

            await UpdateContentItem(updateParams);
        }
        else if (existingProduct == null)
        {
            var addParams = new ContentItemAddParams()
            {
                ContentItem = productSyncItem,
                LanguageName = languageName,
                UserID = userID
            };

            await CreateContentItem(addParams);
        }
    }

    public async Task DeleteNonExistingProducts(IEnumerable<ShopifyProductItem> contentItemProducts, IEnumerable<Product> shopifyProducts, string languageName, int userID)
    {
        var removedProductIDs = await DeleteNonExistingItems(contentItemProducts, shopifyProducts, languageName, userID);
        var childItemsToDelete = new List<int>();
        foreach (var removedProduct in contentItemProducts.Where(x => removedProductIDs.Contains(x.SystemFields.ContentItemID)))
        {
            if (removedProduct == null)
            {
                continue;
            }

            if (removedProduct.Images != null && removedProduct.Images.Any())
            {
                childItemsToDelete.AddRange(removedProduct.Images.Select(x => x.ContentItemIdentifier));
            }
            if (removedProduct.Variants != null && removedProduct.Variants.Any())
            {
                childItemsToDelete.AddRange(removedProduct.Variants.Select(x => x.ContentItemIdentifier));
            }

            var variants = removedProduct.Variants ?? Enumerable.Empty<ShopifyProductVariantItem>();

            foreach (var variant in variants)
            {
                childItemsToDelete.AddRange(variant.Image.Select(x => x.ContentItemIdentifier));
            }
        }

        await contentItemService.DeleteContentItems(childItemsToDelete, languageName, userID);

    }

    private async Task CreateContentItem(ContentItemAddParams addParams)
    {
        if (await contentItemService.AddContentItem(addParams) == 0)
        {
            LogEvent(EventTypeEnum.Error, nameof(ProcessProduct), $"Could not create product {addParams.ContentItem.DisplayName}");
        }
    }

    private async Task UpdateContentItem(ContentItemUpdateParams updateParams)
    {
        if (!await contentItemService.UpdateContentItem(updateParams))
        {
            LogEvent(EventTypeEnum.Error, nameof(UpdateContentItem), $"Could not update product with content item ID {updateParams.ContentItemID}");
        }
    }
}
