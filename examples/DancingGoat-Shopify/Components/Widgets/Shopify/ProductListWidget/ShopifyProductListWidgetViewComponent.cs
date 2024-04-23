using DancingGoat.Components.Widgets.Shopify.ProductListWidget;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Shopify.Models;
using Kentico.Xperience.Shopify.Services.ProductService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using ShopifySharp.GraphQL;

[assembly: RegisterWidget(ShopifyProductListWidgetViewComponent.IDENTIFIER, typeof(ShopifyProductListWidgetViewComponent), "Shopify product list", typeof(ShopifyProductListWidgetProperties), Description = "Displays products from shopify", IconClass = "icon-chain")]
namespace DancingGoat.Components.Widgets.Shopify.ProductListWidget
{
    public class ShopifyProductListWidgetViewComponent : ViewComponent
    {
        public const string IDENTIFIER = "DancingGoat.LandingPage.ShopifyProductList";

        private readonly IShopifyProductService productService;

        public ShopifyProductListWidgetViewComponent(IShopifyProductService productService)
        {
            this.productService = productService;
        }

        public async Task<ViewViewComponentResult> InvokeAsync(ShopifyProductListWidgetProperties properties)
        {
            if (!Enum.TryParse<CurrencyCode>(properties.CurrencyCode, true, out var currencyProperty))
            {
                currencyProperty = CurrencyCode.USD;
            }
            var filter = new ProductFilter()
            {
                Currency = currencyProperty,
                Limit = properties.Limit
            };

            if (long.TryParse(properties.CollectionID, out long collectionID) && collectionID > 0)
            {
                filter.CollectionID = collectionID;
            }

            var products = await productService.GetProductsAsync(filter);

            return View("~/Components/Widgets/Shopify/ProductListWidget/_ShopifyProductListWidget.cshtml", new ShopifyProductListViewModel
            {
                Title = properties.Title,
                Products = products.Items,
                Currency = currencyProperty.ToString()
            });
        }
    }
}
