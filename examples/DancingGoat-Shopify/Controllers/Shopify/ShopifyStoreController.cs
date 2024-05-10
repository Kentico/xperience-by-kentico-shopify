﻿using CMS.ContentEngine;
using CMS.Helpers;
using CMS.Websites;
using CMS.Websites.Routing;
using CMS.Core;
using Shopify.Controllers;
using Microsoft.AspNetCore.Mvc;
using DancingGoat.Models;
using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using DancingGoat;
using Kentico.Xperience.Shopify.Products;
using Kentico.Xperience.Shopify.Products.Models;

[assembly: RegisterWebPageRoute(StorePage.CONTENT_TYPE_NAME, typeof(ShopifyStoreController), WebsiteChannelNames = new[] { DancingGoatConstants.WEBSITE_CHANNEL_NAME })]

namespace Shopify.Controllers;

public class ShopifyStoreController : Controller
{
    private readonly StorePageRepository storePageRepository;
    private readonly IWebPageDataContextRetriever webPageDataContextRetriever;
    private readonly IContentQueryExecutor contentQueryExecutor;
    private readonly IWebPageQueryResultMapper mapper;
    private readonly IWebsiteChannelContext websiteChannelContext;
    private readonly IWebPageUrlRetriever urlRetriever;
    private readonly IProgressiveCache progressiveCache;
    private readonly ISettingsService settingsService;
    private readonly IConversionService conversionService;
    private readonly IShopifyPriceService priceService;

    public ShopifyStoreController(StorePageRepository storePageRepository,
        IWebPageDataContextRetriever webPageDataContextRetriever,
        IContentQueryExecutor contentQueryExecutor,
        IWebPageQueryResultMapper mapper,
        IWebsiteChannelContext websiteChannelContext,
        IWebPageUrlRetriever urlRetriever,
        IProgressiveCache progressiveCache,
        ISettingsService settingsService,
        IConversionService conversionService,
        IShopifyPriceService priceService)
    {
        this.storePageRepository = storePageRepository;
        this.webPageDataContextRetriever = webPageDataContextRetriever;
        this.contentQueryExecutor = contentQueryExecutor;
        this.mapper = mapper;
        this.websiteChannelContext = websiteChannelContext;
        this.urlRetriever = urlRetriever;
        this.progressiveCache = progressiveCache;
        this.settingsService = settingsService;
        this.conversionService = conversionService;
        this.priceService = priceService;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["PageClass"] = "inverted";
        var webPage = webPageDataContextRetriever.Retrieve().WebPage;

        var storePage = await storePageRepository.GetStorePage(webPage.WebPageItemID, webPage.LanguageName, HttpContext.RequestAborted);
        (var bestSellers, var hotTips) = await storePageRepository.GetProducts(storePage, webPage.LanguageName);

        var categories = await GetCategories(storePage, webPage.LanguageName)
            .SelectAwait(x => StoreCategoryListViewModel.GetViewModel(x, urlRetriever))
            .ToListAsync();

        var productPrices = await priceService.GetProductsPrice(
            bestSellers.Concat(hotTips).Select(x => x.Product.FirstOrDefault()?.ShopifyProductID ?? string.Empty));

        var bestSellerModels = await GetProductListViewModels(bestSellers, productPrices);
        var hotTipModels = await GetProductListViewModels(hotTips, productPrices);

        return View(StorePageViewModel.GetViewModel(storePage, categories, hotTipModels, bestSellerModels));
    }

    private async Task<IEnumerable<ShopifyProductListItemViewModel>> GetProductListViewModels(IEnumerable<ProductDetailPage> productPages, IDictionary<string, ProductPriceModel> prices)
    {
        var productViewModels = new List<ShopifyProductListItemViewModel>();
        foreach (var productPage in productPages)
        {
            string shopifyProductId = productPage.Product.FirstOrDefault()?.ShopifyProductID ?? string.Empty;
            var url = await urlRetriever.Retrieve(productPage);

            if (!string.IsNullOrEmpty(shopifyProductId) && prices.TryGetValue(shopifyProductId, out var price))
            {
                productViewModels.Add(ShopifyProductListItemViewModel.GetViewModel(productPage.Product.FirstOrDefault(), url, price));
            }
        }

        return productViewModels;
    }

    private async IAsyncEnumerable<CategoryPage> GetCategories(StorePage store, string languageName)
    {
        string channelName = websiteChannelContext.WebsiteChannelName;
        int cacheMinutes = conversionService.GetInteger(settingsService["CMSCacheMinutes"], 0);
        var categories = await progressiveCache.LoadAsync(async (cacheSettings) =>
        {
            var builder = new ContentItemQueryBuilder()
            .ForContentType(CategoryPage.CONTENT_TYPE_NAME, config =>
                config.ForWebsite(channelName, new PathMatch[]
                {
                    PathMatch.Children(store.SystemFields.WebPageItemTreePath)
                })
                .WithLinkedItems(0))
            .InLanguage(languageName);

            var categories = await contentQueryExecutor.GetWebPageResult(builder, container => mapper.Map<CategoryPage>(container));

            if (cacheSettings.Cached)
            {
                cacheSettings.CacheDependency = CacheHelper.GetCacheDependency(GetDependencyCacheKeys(categories, languageName));
            }

            return categories;
        }, new CacheSettings(cacheMinutes, websiteChannelContext.WebsiteChannelName, nameof(ShopifyStoreController), nameof(GetCategories), languageName, store.SystemFields.WebPageItemGUID));

        foreach (var category in categories)
        {
            yield return category;
        }
    }

    private ISet<string> GetDependencyCacheKeys(IEnumerable<CategoryPage> categories, string languageName)
    {
        var dependencyCacheKeys = new HashSet<string>();

        // Adds cache dependencies on each page in the collection
        foreach (var category in categories)
        {
            // Builds a cache key "webpageitem|byid|<pageId>" for each category
            dependencyCacheKeys.Add(CacheHelper.BuildCacheItemName(new[] { "webpageitem",
                                                                       "byguid",
                                                                       category.SystemFields.WebPageItemGUID.ToString(),
                                                                       languageName},
                                                                           lowerCase: false));

            // Builds a cache key "webpageitem|bychannel|<WebsiteChannelname>|bypath|<pagePath>" for each category
            dependencyCacheKeys.Add(CacheHelper.BuildCacheItemName(new[] { "webpageitem",
                                                                       "bychannel",
                                                                       websiteChannelContext.WebsiteChannelName,
                                                                       "bypath",
                                                                       category.SystemFields.WebPageItemTreePath },
                                                                           lowerCase: false));
        }

        // Creates the cache dependency object from the generated cache keys
        return dependencyCacheKeys;
    }
}
