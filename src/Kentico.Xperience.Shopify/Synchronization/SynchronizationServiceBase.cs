using CMS.Core;

using Kentico.Xperience.Ecommerce.Common.ContentItemSynchronization;
using Kentico.Xperience.Shopify.Synchronization.BulkOperations;
using Kentico.Xperience.Shopify.Synchronization.Variants;

namespace Kentico.Xperience.Shopify.Synchronization;
internal abstract class SynchronizationServiceBase
{
    protected readonly IShopifyContentItemService contentItemService;
    protected readonly IEventLogService eventLogService;

    protected SynchronizationServiceBase(IShopifyContentItemService contentItemService, IEventLogService eventLogService)
    {
        this.contentItemService = contentItemService;
        this.eventLogService = eventLogService;
    }


    /// <summary>
    /// Remove content items from <paramref name="contentItems"/> that are not included in <paramref name="shopifyItems"/>.
    /// </summary>
    /// <param name="contentItems">Existing content items.</param>
    /// <param name="shopifyItems">Items retrieved from Shopify.</param>
    /// <param name="languageName">Content items language.</param>
    /// <param name="userID">User ID used to add/modify/delete content items.</param>
    /// <returns>List of deleted ContentItemIDs</returns>
    protected async Task<IEnumerable<int>> DeleteNonExistingItems(IEnumerable<IContentItemBase>? contentItems, IEnumerable<SynchronizationDtoBase> shopifyItems, string languageName, int userID)
    {
        var removedIDs = new List<int>();
        if (contentItems == null || !contentItems.Any())
        {
            return removedIDs;
        }

        var shopifyObjectIDs = shopifyItems.Select(x => x.Id);

        var contentItemsToDelete = contentItems
            .Where(x => shopifyObjectIDs == null || !shopifyObjectIDs.Contains(x.ShopifyObjectID))
            .Select(x => x.ContentItemIdentifier);

        await contentItemService.DeleteContentItems(contentItemsToDelete, languageName, userID);

        return contentItemsToDelete;
    }


    protected IEnumerable<Guid> OrderItemsByShopify(IEnumerable<IContentItemBase> contentItems, IEnumerable<SynchronizationDtoBase> shopifyObjects)
    {
        foreach (var shopifyObject in shopifyObjects)
        {
            var contentItem = contentItems.FirstOrDefault(x => x.ShopifyObjectID.Equals(shopifyObject.Id, StringComparison.Ordinal));
            yield return contentItem?.SystemFields.ContentItemGUID ?? Guid.Empty;
        }
    }


    protected (IEnumerable<TShopifyItem> ToCreate, IEnumerable<(TShopifyItem ShopifyItem, TContentItem ContentItem)> ToUpdate,
        IEnumerable<TContentItem> ToDelete)
        ClassifyItems<TShopifyItem, TContentItem>(IEnumerable<TShopifyItem> shopifyProducts,
            IEnumerable<TContentItem> existingItems)
        where TShopifyItem : SynchronizationDtoBase
        where TContentItem : IContentItemBase
    {
        var existingLookup = existingItems.ToLookup(item => item.ShopifyObjectID);
        var shopifyLookup = shopifyProducts.ToLookup(item => item.Id.ToString());

        var toCreate = shopifyProducts.Where(shopifyItem => !existingLookup.Contains(shopifyItem.Id))
            .ToList();

        var toUpdate = shopifyProducts.SelectMany(storeItem => existingLookup[storeItem.Id],
                (storeItem, existingItem) => (storeItem, existingItem))
            .ToList();

        var toDelete = existingItems.Where(p => !shopifyLookup.Contains(p.ShopifyObjectID)).ToList();

        return (toCreate, toUpdate, toDelete);
    }


    protected void LogEvent(EventTypeEnum eventType, string eventCode, string description) =>
        eventLogService.LogEvent(eventType, nameof(IVariantSynchronizationService), eventCode, description);
}
