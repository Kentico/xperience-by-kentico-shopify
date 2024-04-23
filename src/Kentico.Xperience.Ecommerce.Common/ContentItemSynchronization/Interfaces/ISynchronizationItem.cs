using Kentico.Xperience.Ecommerce.Common.ContentItemSynchronization;

namespace Kentico.Xperience.Shopify.ProductSynchronization;

public interface ISynchronizationItem<TContentItem>
    where TContentItem : IContentItemBase, new()
{
    /// <summary>
    /// Fills modifiedProps with modified properties compared to contentItem
    /// </summary>
    /// <param name="contentItem"></param>
    /// <param name="modifiedProps"></param>
    /// <returns>True if any property was modified. Otherwise False</returns>
    public bool GetModifiedProperties(TContentItem contentItem, out Dictionary<string, object?> modifiedProps);
}
