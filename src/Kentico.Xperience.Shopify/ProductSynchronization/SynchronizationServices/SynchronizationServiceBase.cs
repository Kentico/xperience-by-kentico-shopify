using CMS.Core;
using Kentico.Xperience.Ecommerce.Common.ContentItemSynchronization;
using ShopifySharp;

namespace Kentico.Xperience.Shopify.ProductSynchronization;
public abstract class SynchronizationServiceBase
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
    /// <param name="contentItems"></param>
    /// <param name="shopifyItems"></param>
    /// <param name="languageName"></param>
    /// <param name="userID"></param>
    /// <returns>List of deleted ContentItemIDs</returns>
    protected async Task<IEnumerable<int>> DeleteNonExistingItems(IEnumerable<IContentItemBase>? contentItems, IEnumerable<ShopifyObject> shopifyItems, string languageName, int userID)
    {
        var removedIDs = new List<int>();
        if (contentItems == null || !contentItems.Any())
        {
            return removedIDs;
        }

        var shopifyObjectIDs = shopifyItems.Where(x => x.Id.HasValue).Select(x => x.Id.ToString() ?? string.Empty);

        var contentItemsToDelete = contentItems
            .Where(x => shopifyObjectIDs == null || !shopifyObjectIDs.Contains(x.ShopifyObjectID))
            .Select(x => x.ContentItemIdentifier);

        await contentItemService.DeleteContentItems(contentItemsToDelete, languageName, userID);

        return contentItemsToDelete;
    }

    protected IEnumerable<Guid> OrderItemsByShopify(IEnumerable<IContentItemBase> contentItems, IOrderedEnumerable<ShopifyObject> shopifyObjects)
    {
        foreach (var shopifyObject in shopifyObjects.Where(x => x.Id.HasValue))
        {
            var contentItem = contentItems.FirstOrDefault(x => x.ShopifyObjectID == (shopifyObject.Id?.ToString() ?? string.Empty));
            yield return contentItem?.SystemFields.ContentItemGUID ?? Guid.Empty;
        }
    }

    protected void LogEvent(EventTypeEnum eventType, string eventCode, string description) =>
        eventLogService.LogEvent(eventType, nameof(IVariantSynchronizationService), eventCode, description);
}
