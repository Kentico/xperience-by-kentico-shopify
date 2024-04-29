using DancingGoat.Components.ViewComponents.ShopifyCartWidget;
using Kentico.Xperience.Shopify.ShoppingCart;
using Microsoft.AspNetCore.Mvc;

namespace DancingGoat.Components.ViewComponents
{
    public class ShopifyCartViewComponent : ViewComponent
    {
        private readonly IShoppingService shoppingService;

        public ShopifyCartViewComponent(
            IShoppingService shoppingService)
        {
            this.shoppingService = shoppingService;
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
    }
}
