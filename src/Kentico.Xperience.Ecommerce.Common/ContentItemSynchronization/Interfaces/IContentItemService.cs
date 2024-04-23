using CMS.ContentEngine;

namespace Kentico.Xperience.Ecommerce.Common.ContentItemSynchronization;

public interface IContentItemService
{
    /// <summary>
    /// Add and publish content item.
    /// </summary>
    /// <param name="addParams"></param>
    /// <returns>ID of created content item.</returns>
    Task<int> AddContentItem(ContentItemAddParams addParams);

    Task<IEnumerable<T>> GetContentItems<T>(string contentType, Action<ContentTypeQueryParameters> queryParams)
        where T : IContentItemFieldsSource, new();

    Task<IEnumerable<T>> GetContentItems<T>(string contentType)
        where T : IContentItemFieldsSource, new();

    Task<IEnumerable<T>> GetContentItems<T>(string contentType, int linkedItemsLevel)
         where T : IContentItemFieldsSource, new();

    Task<bool> UpdateContentItem(ContentItemUpdateParams updateParams);

    Task DeleteContentItem(int contentItemID, string languageName, int userID);

    Task DeleteContentItems(IEnumerable<int> contentItemIDs, string languageName, int userID);
}

