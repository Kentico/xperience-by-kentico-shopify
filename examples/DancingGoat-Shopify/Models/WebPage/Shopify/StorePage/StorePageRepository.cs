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
        IProgressiveCache cache,
        IWebPageLinkedItemsDependencyAsyncRetriever webPageLinkedItemsDependencyRetriever,
        ISettingsService settingsService,
        IConversionService conversionService)
        : base(websiteChannelContext, executor, cache, webPageLinkedItemsDependencyRetriever)
    {
        this.settingsService = settingsService;
        this.conversionService = conversionService;
    }

    /// <summary>
    /// Returns <see cref="StorePage"/> content item.
    /// </summary>
    public async Task<StorePage> GetStorePage(int webPageItemId, string languageName, CancellationToken cancellationToken = default)
    {
        var queryBuilder = GetQueryBuilder(webPageItemId, languageName, StorePage.CONTENT_TYPE_NAME, 2);
        int cacheMinutes = conversionService.GetInteger(settingsService["CMSCacheMinutes"], 0);

        var cacheSettings = new CacheSettings(cacheMinutes, WebsiteChannelContext.WebsiteChannelName, nameof(StorePage), webPageItemId, languageName);

        var result = await GetCachedQueryResult<StorePage>(queryBuilder, null, cacheSettings, (pages, token) => GetDependencyCacheKeys(pages, 0, token), cancellationToken);

        return result.FirstOrDefault();
    }


    public async Task<(IEnumerable<ProductDetailPage> BestSellers, IEnumerable<ProductDetailPage> HotTips)> GetProducts(StorePage storePage, string languageName, CancellationToken cancellationToken = default)
    {
        var bestSellerGuids = storePage.Bestsellers.Select(x => x.WebPageGuid);
        var hotTipsGuids = storePage.HotTips.Select(x => x.WebPageGuid);

        var allLinkedPagesGuids = bestSellerGuids.Concat(hotTipsGuids).ToArray();
        int cacheMinutes = conversionService.GetInteger(settingsService["CMSCacheMinutes"], 0);

        var queryBuilder = new ContentItemQueryBuilder()
                    .ForContentType(ProductDetailPage.CONTENT_TYPE_NAME,
                        config => config
                                .WithLinkedItems(2)
                                .ForWebsite(WebsiteChannelContext.WebsiteChannelName)
                                .Where(where => where.WhereIn(nameof(IWebPageContentQueryDataContainer.WebPageItemGUID), allLinkedPagesGuids)))
                    .InLanguage(languageName);
        var cacheSettings = new CacheSettings(cacheMinutes, WebsiteChannelContext.WebsiteChannelName, nameof(StorePage), storePage.SystemFields.WebPageItemGUID, languageName, "relatedProducts");
        var allLinkedPages = await GetCachedQueryResult<ProductDetailPage>(queryBuilder, null, cacheSettings, (pages, token) => GetProductsDependencyCacheKeys(pages, cancellationToken), cancellationToken);

        var bestSellers = new List<ProductDetailPage>();
        var hotTips = new List<ProductDetailPage>();
        foreach (var page in allLinkedPages)
        {
            var pageGuid = page.SystemFields.WebPageItemGUID;
            if (hotTipsGuids.Contains(pageGuid))
            {
                hotTips.Add(page);
            }
            if (bestSellerGuids.Contains(pageGuid))
            {
                bestSellers.Add(page);
            }
        }

        return (bestSellers, hotTips);
    }

    private async Task<ISet<string>> GetProductsDependencyCacheKeys(IEnumerable<ProductDetailPage> productPages, CancellationToken cancellationToken)
    {

        if (productPages == null)
        {
            return await Task.FromResult(new HashSet<string>());
        }

        var result = new List<string>();
        foreach (var productPage in productPages)
        {
            result.AddRange(await webPageLinkedItemsDependencyRetriever.Get(productPage.SystemFields.WebPageItemID, 1, cancellationToken));
            result.Add(CacheHelper.BuildCacheItemName(new[] { "webpageitem", "byid", productPage.SystemFields.WebPageItemID.ToString() }, false));
        }

        return await Task.FromResult(result.ToHashSet());
    }
}
