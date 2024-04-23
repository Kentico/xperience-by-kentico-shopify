using CMS.ContentEngine;
using CMS.Core;
using CMS.Helpers;
using CMS.Websites;
using CMS.Websites.Routing;

namespace DancingGoat.Models;

public class StorePageRepository : ShopifyContentRepositoryBase
{
    private readonly ISettingsService settingsService;
    private readonly IConversionService conversionService;

    public StorePageRepository(
        IWebsiteChannelContext websiteChannelContext,
        IContentQueryExecutor executor,
        IWebPageQueryResultMapper mapper,
        IProgressiveCache cache,
        IWebPageLinkedItemsDependencyAsyncRetriever webPageLinkedItemsDependencyRetriever,
        ISettingsService settingsService,
        IConversionService conversionService)
        : base(websiteChannelContext, executor, mapper, cache, webPageLinkedItemsDependencyRetriever)
    {
        this.settingsService = settingsService;
        this.conversionService = conversionService;
    }

    /// <summary>
    /// Returns <see cref="StorePage"/> content item.
    /// </summary>
    public async Task<StorePage> GetStorePage(int webPageItemId, string languageName, CancellationToken cancellationToken = default)
    {
        var queryBuilder = GetQueryBuilder(webPageItemId, languageName, StorePage.CONTENT_TYPE_NAME, 1);
        int cacheMinutes = conversionService.GetInteger(settingsService["CMSCacheMinutes"], 0);

        var cacheSettings = new CacheSettings(cacheMinutes, WebsiteChannelContext.WebsiteChannelName, nameof(StorePage), webPageItemId, languageName);

        var result = await GetCachedQueryResult<StorePage>(queryBuilder, null, cacheSettings, (pages, token) => GetDependencyCacheKeys(pages, 0, token), cancellationToken);

        return result.FirstOrDefault();
    }
}
