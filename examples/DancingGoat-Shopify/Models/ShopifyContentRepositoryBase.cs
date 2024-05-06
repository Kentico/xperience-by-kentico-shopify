using CMS.ContentEngine;
using CMS.Helpers;
using CMS.Websites;
using CMS.Websites.Routing;

namespace DancingGoat.Models
{
    public class ShopifyContentRepositoryBase : ContentRepositoryBase
    {
        protected readonly IWebPageLinkedItemsDependencyAsyncRetriever webPageLinkedItemsDependencyRetriever;

        public ShopifyContentRepositoryBase(IWebsiteChannelContext websiteChannelContext, IContentQueryExecutor executor, IProgressiveCache cache, IWebPageLinkedItemsDependencyAsyncRetriever webPageLinkedItemsDependencyRetriever)
            : base(websiteChannelContext, executor, cache)
        {
            this.webPageLinkedItemsDependencyRetriever = webPageLinkedItemsDependencyRetriever;
        }

        protected async Task<ISet<string>> GetDependencyCacheKeys(IEnumerable<IWebPageFieldsSource> pages, int maxLevel, CancellationToken cancellationToken)
        {
            var page = pages.FirstOrDefault();

            if (page == null)
            {
                return new HashSet<string>();
            }

            return (await webPageLinkedItemsDependencyRetriever.Get(page.SystemFields.WebPageItemID, maxLevel, cancellationToken))
                .Append(CacheHelper.BuildCacheItemName(new[] { "webpageitem", "byid", page.SystemFields.WebPageItemID.ToString() }, false))
                .Append(CacheHelper.GetCacheItemName(null, WebsiteChannelInfo.OBJECT_TYPE, "byid", WebsiteChannelContext.WebsiteChannelID))
                .Append(CacheHelper.GetCacheItemName(null, ContentLanguageInfo.OBJECT_TYPE, "all"))
                .ToHashSet(StringComparer.InvariantCultureIgnoreCase);
        }

        protected ContentItemQueryBuilder GetQueryBuilder(int webPageItemId, string languageName, string contentTypeName, int linkedItemsLevel)
        {
            return new ContentItemQueryBuilder()
                    .ForContentType(contentTypeName,
                        config => config
                                .WithLinkedItems(linkedItemsLevel)
                                .ForWebsite(WebsiteChannelContext.WebsiteChannelName)
                                .Where(where => where.WhereEquals(nameof(IWebPageContentQueryDataContainer.WebPageItemID), webPageItemId))
                                .TopN(1))
                    .InLanguage(languageName);
        }
    }
}
