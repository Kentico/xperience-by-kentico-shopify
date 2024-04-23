using CMS.ContentEngine;
using CMS.Core;
using CMS.Helpers;
using CMS.Websites;
using CMS.Websites.Routing;

namespace DancingGoat.Models;

public class ProductDetailPageRepository : ShopifyContentRepositoryBase
{
    private readonly ISettingsService settingsService;
    private readonly IConversionService conversionService;

    public ProductDetailPageRepository(
        IWebsiteChannelContext websiteChannelContext,
        IContentQueryExecutor executor,
        IWebPageQueryResultMapper mapper,
        IProgressiveCache cache,
        IWebPageLinkedItemsDependencyAsyncRetriever webPageLinkedItemsDependencyRetriever,
        ISettingsService settingsService,
        IConversionService conversionService)
        : base(websiteChannelContext, executor, mapper, cache, webPageLinkedItemsDependencyRetriever)
    {
        this.conversionService = conversionService;
        this.settingsService = settingsService;
    }

    /// <summary>
    /// Returns <see cref="ProductDetailPage"/> content item.
    /// </summary>
    public async Task<ProductDetailPage> GetProductDetailPage(int webPageItemId, string languageName, CancellationToken cancellationToken = default)
    {
        var queryBuilder = GetQueryBuilder(webPageItemId, languageName, ProductDetailPage.CONTENT_TYPE_NAME, 3);
        int cacheMinutes = conversionService.GetInteger(settingsService["CMSCacheMinutes"], 0);
        var cacheSettings = new CacheSettings(cacheMinutes, WebsiteChannelContext.WebsiteChannelName, nameof(ProductDetailPage), webPageItemId, languageName);

        var result = await GetCachedQueryResult<ProductDetailPage>(queryBuilder, null, cacheSettings, (pages, token) => GetDependencyCacheKeys(pages, 1, token), cancellationToken);

        return result.FirstOrDefault();
    }
}
