using CMS.Websites;
using DancingGoat;
using DancingGoat.Controllers.Shopify;
using DancingGoat.Models;
using DancingGoat.Models.WebPage.Shopify.ShoppingCartPage;
using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.Xperience.Shopify.ShoppingCart;
using Kentico.Xperience.Shopify.Synchronization;
using Microsoft.AspNetCore.Mvc;
using Shopify;
using Shopify.ContentTypes;

[assembly: RegisterWebPageRoute(ShoppingCartPage.CONTENT_TYPE_NAME, typeof(ShopifyShoppingCartController), WebsiteChannelNames = new[] { DancingGoatConstants.WEBSITE_CHANNEL_NAME })]


namespace DancingGoat.Controllers.Shopify
{
    public class ShopifyShoppingCartController : Controller
    {
        private readonly IWebPageDataContextRetriever webPageDataContextRetriever;
        private readonly IShoppingService shoppingService;
        private readonly IWebPageUrlRetriever webPageUrlRetriever;
        private readonly IShopifyContentItemService contentItemService;

        public ShopifyShoppingCartController(
            IWebPageDataContextRetriever webPageDataContextRetriever,
            IShoppingService shoppingService,
            IWebPageUrlRetriever webPageUrlRetriever,
            IShopifyContentItemService contentItemService)
        {
            this.webPageDataContextRetriever = webPageDataContextRetriever;
            this.shoppingService = shoppingService;
            this.webPageUrlRetriever = webPageUrlRetriever;
            this.contentItemService = contentItemService;
        }


        public async Task<IActionResult> Index()
        {
            var cart = await shoppingService.GetCurrentShoppingCart();
            var images = await GetCartItemsImages(cart.Items.Select(x => x.VariantGraphQLId));
            var model = ShoppingCartContentViewModel.GetViewModel(cart, images);

            return View("~/Views/ShopifyShoppingCart/Index.cshtml", model);
        }


        private async Task<Dictionary<string, string>> GetCartItemsImages(IEnumerable<string> variantGraphQLIds)
        {
            var variants = await contentItemService.GetVariants(variantGraphQLIds.ToArray());
            var products = await contentItemService.GetContentItems<ShopifyProductItem>(
                Product.CONTENT_TYPE_NAME,
                q => q.Where(x => x.WhereIn(nameof(ShopifyProductItem.ShopifyProductID), variants.Select(x => x.ShopifyProductID).ToArray()))
                    .Columns(nameof(ShopifyProductItem.ShopifyProductID), nameof(ShopifyProductItem.Images))
                    .WithLinkedItems(1));

            var productsDict = products.ToLookup(x => x.ShopifyProductID, x => x.Images.FirstOrDefault()?.ImageAsset.Url);

            return variants.ToDictionary(x => x.ShopifyMerchandiseID, x => productsDict[x.ShopifyProductID].FirstOrDefault());
        }
    }
}
