using CMS.ContentEngine;
using CMS.Core;
using CMS.Helpers;
using CMS.Websites;
using CMS.Websites.Routing;
using DancingGoat.Components.ViewComponents.ShopifyCartWidget;
using DancingGoat.Models;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.Xperience.Shopify.ShoppingCart;
using Microsoft.AspNetCore.Mvc;

namespace DancingGoat.Components.ViewComponents
{
    public class ShopifyCartViewComponent : ViewComponent
    {
        private readonly IShoppingService shoppingService;
        private readonly IPreferredLanguageRetriever preferredLanguageRetriever;
        private readonly IWebsiteChannelContext websiteChannelContext;
        private readonly IContentQueryExecutor executor;
        private readonly IWebPageUrlRetriever urlRetriever;
        private readonly IWebPageQueryResultMapper queryResultMapper;
        private readonly IProgressiveCache progressiveCache;
        private readonly ISettingsService settingsService;
        private readonly IConversionService conversionService;

        public ShopifyCartViewComponent(
            IShoppingService shoppingService,
            IPreferredLanguageRetriever preferredLanguageRetriever,
            IWebsiteChannelContext websiteChannelContext,
            IContentQueryExecutor executor,
            IWebPageUrlRetriever urlRetriever,
            IWebPageQueryResultMapper queryResultMapper,
            IProgressiveCache progressiveCache,
            IConversionService conversionService,
            ISettingsService settingsService)
        {
            this.shoppingService = shoppingService;
            this.websiteChannelContext = websiteChannelContext;
            this.preferredLanguageRetriever = preferredLanguageRetriever;
            this.executor = executor;
            this.urlRetriever = urlRetriever;
            this.queryResultMapper = queryResultMapper;
            this.progressiveCache = progressiveCache;
            this.conversionService = conversionService;
            this.settingsService = settingsService;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cart = await shoppingService.GetCurrentShoppingCart();
            var model = new ShopifyCartWidgetViewModel()
            {
                CartItemCount = cart?.Items.Sum(x => x.Quantity) ?? 0,
                CartUrl = DancingGoatConstants.SHOPPING_CART_PATH
            };

            return View($"~/Components/ViewComponents/ShopifyCartWidget/Default.cshtml", model);
        }

        private async Task<string> GetShoppingCartPageUrl()
        {
            string websiteChannelName = websiteChannelContext.WebsiteChannelName;
            string preferredLanguage = preferredLanguageRetriever.Get();
            int cacheMinutes = conversionService.GetInteger(settingsService["CMSCacheMinutes"], 0);

            return await progressiveCache.LoadAsync(async (cs) =>
            {
                string test = websiteChannelName + preferredLanguage;
                int test2 = cacheMinutes;
                var builder = new ContentItemQueryBuilder()
                            .ForContentType(
                                ShoppingCartPage.CONTENT_TYPE_NAME,
                                config => config
                                    .ForWebsite(websiteChannelName)
                                    .TopN(1)
                            ).InLanguage(preferredLanguage);

                var cartPage = (await executor.GetWebPageResult(builder, queryResultMapper.Map<ShoppingCartPage>)).FirstOrDefault();

                cs.CacheDependency = CacheHelper.GetCacheDependency($"webpageitem|byid|{cartPage?.SystemFields.WebPageItemID}");

                return cartPage == null ? null : (await urlRetriever.Retrieve(cartPage))?.RelativePath;
            },
            new CacheSettings(cacheMinutes, nameof(ShopifyCartViewComponent), $"|{preferredLanguage}.{websiteChannelName}.CartPageUrl"));
        }
    }
}
