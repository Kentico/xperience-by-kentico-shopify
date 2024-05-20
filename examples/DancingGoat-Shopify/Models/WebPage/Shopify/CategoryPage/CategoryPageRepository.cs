using CMS.ContentEngine;
using CMS.Core;
using CMS.Helpers;
using CMS.Websites;
using CMS.Websites.Routing;

namespace DancingGoat.Models;

public class CategoryPageRepository : ShopifyContentRepositoryBase
{
    private readonly ISettingsService settingsService;
    private readonly IConversionService conversionService;

    public CategoryPageRepository(
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
    /// Returns <see cref="CategoryPage"/> content item.
    /// </summary>
    public async Task<CategoryPage> GetCategoryPage(int webPageItemId, string languageName, CancellationToken cancellationToken = default)
    {
        var queryBuilder = GetQueryBuilder(webPageItemId, languageName, CategoryPage.CONTENT_TYPE_NAME, 4);
        int cacheMinutes = conversionService.GetInteger(settingsService["CMSCacheMinutes"], 0);
        var cacheSettings = new CacheSettings(cacheMinutes, WebsiteChannelContext.WebsiteChannelName, nameof(CategoryPage), languageName, webPageItemId);

        var result = await GetCachedQueryResult<CategoryPage>(queryBuilder, null, cacheSettings, (pages, token) => GetDependencyCacheKeys(pages, 1, token), cancellationToken);

        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<ProductDetailPage>> GetCategoryProducts(CategoryPage category, string languageName, CancellationToken cancellationToken = default)
    {
        var queryBuilder = GetProductsQueryBuilder(category.CategoryProducts.Select(x => x.WebPageGuid).ToArray(), languageName);
        int cacheMinutes = conversionService.GetInteger(settingsService["CMSCacheMinutes"], 0);

        var cacheSettings = new CacheSettings(cacheMinutes, WebsiteChannelContext.WebsiteChannelName, nameof(ProductDetailPage), languageName, category.SystemFields.WebPageItemID);
        return await GetCachedQueryResult<ProductDetailPage>(queryBuilder, null, cacheSettings, GetProductsDependencyCacheKeys, cancellationToken);
    }

    private ContentItemQueryBuilder GetProductsQueryBuilder(Guid[] webPageItemGUIDs, string languageName)
    {
        return new ContentItemQueryBuilder()
                .ForContentType(ProductDetailPage.CONTENT_TYPE_NAME,
                    config => config
                            .WithLinkedItems(4)
                            .ForWebsite(WebsiteChannelContext.WebsiteChannelName)
                            .Where(where => where.WhereIn(nameof(IWebPageContentQueryDataContainer.WebPageItemGUID), webPageItemGUIDs)))
                .InLanguage(languageName);
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
