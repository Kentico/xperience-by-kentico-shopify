using CMS.ContentEngine;

namespace Kentico.Xperience.Ecommerce.Common.ContentItemSynchronization;

public class ContentItemServiceBase : IContentItemService
{
    protected readonly IContentItemManagerFactory contentItemManagerFactory;
    protected readonly IContentQueryExecutor contentQueryExecutor;
    protected readonly IContentQueryResultMapper contentQueryResultMapper;

    public ContentItemServiceBase(
        IContentItemManagerFactory contentItemManagerFactory,
        IContentQueryExecutor contentQueryExecutor,
        IContentQueryResultMapper contentQueryResultMapper)
    {
        this.contentItemManagerFactory = contentItemManagerFactory;
        this.contentQueryExecutor = contentQueryExecutor;
        this.contentQueryResultMapper = contentQueryResultMapper;
    }

    public async Task<int> AddContentItem(ContentItemAddParams addParams)
    {
        ArgumentNullException.ThrowIfNull(addParams);

        var createParams = new CreateContentItemParameters(
                                            addParams.ContentItem.ContentTypeName,
                                            addParams.ContentItem.GenerateCodeName(),
                                            addParams.ContentItem.DisplayName,
                                            addParams.LanguageName);
        var itemData = new ContentItemData(addParams.ContentItem.ToDict());

        var contentItemManager = contentItemManagerFactory.Create(addParams.UserID);
        int itemID = await contentItemManager.Create(createParams, itemData);
        await contentItemManager.TryPublish(itemID, addParams.LanguageName);

        return itemID;
    }

    public async Task<bool> UpdateContentItem(ContentItemUpdateParams updateParams)
    {
        var contentItemManager = contentItemManagerFactory.Create(updateParams.UserID);
        var versionStatus = updateParams.VersionStatus;

        // If content item version is not draft, create draft in order to edit it
        if (versionStatus != VersionStatus.Draft &&
            versionStatus != VersionStatus.InitialDraft &&
            !await contentItemManager.TryCreateDraft(updateParams.ContentItemID, updateParams.LanguageName))
        {
            return false;
        }

        var itemData = new ContentItemData(updateParams.ContentItemParams);
        if (!await contentItemManager.TryUpdateDraft(updateParams.ContentItemID, updateParams.LanguageName, itemData))
        {
            return false;
        }

        return versionStatus switch
        {
            VersionStatus.Published => await contentItemManager.TryPublish(updateParams.ContentItemID, updateParams.LanguageName),
            VersionStatus.Archived => await contentItemManager.TryArchive(updateParams.ContentItemID, updateParams.LanguageName),
            VersionStatus.InitialDraft => true,
            VersionStatus.Draft => true,
            _ => true,
        };
    }

    public async Task<IEnumerable<T>> GetContentItems<T>(string contentType, Action<ContentTypeQueryParameters> queryParams)
        where T : IContentItemFieldsSource, new()
    {
        var builder = new ContentItemQueryBuilder()
            .ForContentType(contentType, queryParams);

        return await contentQueryExecutor.GetResult(builder, contentQueryResultMapper.Map<T>);
    }

    public async Task<IEnumerable<T>> GetContentItems<T>(string contentType)
         where T : IContentItemFieldsSource, new()
    {
        return await GetContentItems<T>(contentType, 0);
    }

    public async Task<IEnumerable<T>> GetContentItems<T>(string contentType, int linkedItemsLevel)
        where T : IContentItemFieldsSource, new()
    {
        var builder = new ContentItemQueryBuilder()
            .ForContentType(contentType, config => config.WithLinkedItems(linkedItemsLevel));

        return await contentQueryExecutor.GetResult(builder, contentQueryResultMapper.Map<T>);
    }

    public async Task DeleteContentItem(int contentItemID, string languageName, int userID)
    {
        var contentItemManager = contentItemManagerFactory.Create(userID);
        await contentItemManager.Delete(contentItemID, languageName);
    }

    public async Task DeleteContentItems(IEnumerable<int> contentItemIDs, string languageName, int userID)
    {
        if (contentItemIDs == null || !contentItemIDs.Any())
        {
            return;
        }

        var contentItemManager = contentItemManagerFactory.Create(userID);

        foreach (int contentItemID in contentItemIDs)
        {
            await contentItemManager.Delete(contentItemID, languageName);
        }
    }
}

