using CMS.Websites.Routing;
using CMS.Websites;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.Xperience.Shopify.ShoppingCart;
using Microsoft.AspNetCore.Mvc;

namespace DancingGoat.Components.ViewComponents.ShopifyCart
{
    public class ShopifyCartViewComponent : ViewComponent
    {
        private readonly IShoppingService shoppingService;
        private readonly IWebPageUrlRetriever webPageUrlRetriever;
        private readonly IWebsiteChannelContext websiteChannelContext;
        private readonly IPreferredLanguageRetriever currentLanguageRetriever;

        public ShopifyCartViewComponent(
            IShoppingService shoppingService,
            IWebPageUrlRetriever webPageUrlRetriever,
            IWebsiteChannelContext websiteChannelContext,
            IPreferredLanguageRetriever currentLanguageRetriever)
        {
            this.shoppingService = shoppingService;
            this.webPageUrlRetriever = webPageUrlRetriever;
            this.websiteChannelContext = websiteChannelContext;
            this.currentLanguageRetriever = currentLanguageRetriever;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cart = await shoppingService.GetCurrentShoppingCart();
            string language = currentLanguageRetriever.Get();
            string websiteChannelName = websiteChannelContext.WebsiteChannelName;

            var model = new ShopifyCartViewComponentModel()
            {
                CartItemCount = cart?.Items.Sum(x => x.Quantity) ?? 0,
                CartUrl = (await webPageUrlRetriever.Retrieve(DancingGoatConstants.SHOPPING_CART_PATH, websiteChannelName, language)).RelativePath
            };

            return View($"~/Components/ViewComponents/ShopifyCart/Default.cshtml", model);
        }
    }
}
