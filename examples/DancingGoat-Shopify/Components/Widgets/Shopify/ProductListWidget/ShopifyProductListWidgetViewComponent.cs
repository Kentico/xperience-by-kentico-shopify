using DancingGoat.Components.Widgets.Shopify.ProductListWidget;

using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Shopify.Config;
using Kentico.Xperience.Shopify.Products;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

using ShopifySharp.GraphQL;

[assembly: RegisterWidget(ShopifyProductListWidgetViewComponent.IDENTIFIER, typeof(ShopifyProductListWidgetViewComponent), "Shopify product list", typeof(ShopifyProductListWidgetProperties), Description = "Displays products from shopify", IconClass = "icon-chain")]
namespace DancingGoat.Components.Widgets.Shopify.ProductListWidget
{
    public class ShopifyProductListWidgetViewComponent : ViewComponent
    {
        public const string IDENTIFIER = "DancingGoat.LandingPage.ShopifyProductList";

        private readonly IShopifyCollectionService collectionService;
        private readonly IShopifyProductService productService;
        private readonly IShopifyIntegrationSettingsService shopifyIntegrationSettingsService;

        public ShopifyProductListWidgetViewComponent(
            IShopifyCollectionService collectionService,
            IShopifyProductService productService,
            IShopifyIntegrationSettingsService shopifyIntegrationSettingsService)
        {
            this.collectionService = collectionService;
            this.productService = productService;
            this.shopifyIntegrationSettingsService = shopifyIntegrationSettingsService;
        }

        public async Task<ViewViewComponentResult> InvokeAsync(ShopifyProductListWidgetProperties properties)
        {
            if (!Enum.TryParse<CurrencyCode>(properties.CurrencyCode, true, out var currencyProperty))
            {
                currencyProperty = CurrencyCode.USD;
            }

            if (properties.Limit <= 0)
            {
                properties.Limit = 250;
            }

            var settings = shopifyIntegrationSettingsService.GetWebsiteChannelSettings();

            var products = string.IsNullOrEmpty(properties.CollectionID)
                ? (await productService.GetProductsAsync(new Kentico.Xperience.Shopify.Products.Models.ProductFilter())).Items
                : await collectionService.GetCollectionProducts(properties.CollectionID, properties.Limit, settings.CountryCode);

            return View("~/Components/Widgets/Shopify/ProductListWidget/_ShopifyProductListWidget.cshtml", new ShopifyProductListViewModel
            {
                Title = properties.Title,
                Products = products,
                Currency = currencyProperty.ToString()
            });
        }
    }
}
