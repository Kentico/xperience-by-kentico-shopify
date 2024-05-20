using CMS.Core;
using CMS.Helpers;
using CMS.Websites;

using DancingGoat;
using DancingGoat.Models;

using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.Xperience.Shopify.Products;
using Kentico.Xperience.Shopify.Products.Models;

using Microsoft.AspNetCore.Mvc;

using Shopify.Controllers;

[assembly: RegisterWebPageRoute(CategoryPage.CONTENT_TYPE_NAME, typeof(ShopifyCategoryController), WebsiteChannelNames = new[] { DancingGoatConstants.WEBSITE_CHANNEL_NAME })]

namespace Shopify.Controllers;

public class ShopifyCategoryController : Controller
{
    private readonly CategoryPageRepository categoryPageRepository;
    private readonly IWebPageDataContextRetriever webPageDataContextRetriever;
    private readonly IWebPageUrlRetriever urlRetriever;
    private readonly IShopifyPriceService priceService;
    private readonly IProgressiveCache progressiveCache;
    private readonly ISettingsService settingsService;
    private readonly IConversionService conversionService;
    private readonly ILogger<ShopifyCategoryController> logger;

    public ShopifyCategoryController(
        CategoryPageRepository categoryPageRepository,
        IWebPageDataContextRetriever webPageDataContextRetriever,
        IWebPageUrlRetriever urlRetriever,
        IShopifyPriceService priceService,
        IProgressiveCache progressiveCache,
        ISettingsService settingsService,
        IConversionService conversionService,
        ILogger<ShopifyCategoryController> logger)
    {
        this.categoryPageRepository = categoryPageRepository;
        this.webPageDataContextRetriever = webPageDataContextRetriever;
        this.urlRetriever = urlRetriever;
        this.priceService = priceService;
        this.progressiveCache = progressiveCache;
        this.settingsService = settingsService;
        this.conversionService = conversionService;
        this.logger = logger;
    }
    public async Task<IActionResult> Index()
    {
        var webPage = webPageDataContextRetriever.Retrieve().WebPage;
        var categoryPage = await categoryPageRepository.GetCategoryPage(webPage.WebPageItemID, webPage.LanguageName, HttpContext.RequestAborted);
        var products = (await categoryPageRepository.GetCategoryProducts(categoryPage, webPage.LanguageName, HttpContext.RequestAborted))
            .Where(x => x.Product != null && x.Product.Any());
        var productGuids = products.Select(x => x.SystemFields.WebPageItemGUID).ToList().AsReadOnly();
        var urls = await urlRetriever.Retrieve(productGuids, webPage.WebsiteChannelName, webPage.LanguageName);
        int cacheMinutes = conversionService.GetInteger(settingsService["CMSCacheMinutes"], 0);

        var prices = await progressiveCache.LoadAsync(
            async (_) => await GetProductPrices(products.Select(x => x.Product.FirstOrDefault())),
            new CacheSettings(cacheMinutes, webPage.WebsiteChannelName, webPage.LanguageName, categoryPage.SystemFields.WebPageItemGUID));

        return View(CategoryPageViewModel.GetViewModel(categoryPage, prices, products, urls, logger));
    }

    private async Task<IDictionary<string, ProductPriceModel>> GetProductPrices(IEnumerable<Product> productContentItems)
    {
        var productIds = productContentItems.Select(x => x.ShopifyProductID);
        return await priceService.GetProductsPrice(productIds);
    }
}
