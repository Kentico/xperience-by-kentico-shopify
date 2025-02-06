using DancingGoat.Components.Widgets.Shopify.ProductListWidget;

using Kentico.PageBuilder.Web.Mvc;
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

        public ShopifyProductListWidgetViewComponent(
            IShopifyCollectionService collectionService,
            IShopifyProductService productService)
        {
            this.collectionService = collectionService;
            this.productService = productService;
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

            // TODO use country code from filter
            var products = string.IsNullOrEmpty(properties.CollectionID)
                ? (await productService.GetProductsAsync(new Kentico.Xperience.Shopify.Products.Models.ProductFilter())).Items
                : await collectionService.GetCollectionProducts(properties.CollectionID, properties.Limit, CountryCode.US);

            return View("~/Components/Widgets/Shopify/ProductListWidget/_ShopifyProductListWidget.cshtml", new ShopifyProductListViewModel
            {
                Title = properties.Title,
                Products = products,
                Currency = currencyProperty.ToString()
            });
        }
    }
}
