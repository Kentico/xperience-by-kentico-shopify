using DancingGoat;
using DancingGoat.Controllers.Shopify;
using DancingGoat.Models;
using DancingGoat.Models.WebPage.Shopify.ShoppingCartPage;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.Xperience.Shopify.Products.Models;
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
        private readonly IShoppingService shoppingService;
        private readonly IShopifyContentItemService contentItemService;

        public ShopifyShoppingCartController(
            IShoppingService shoppingService,
            IShopifyContentItemService contentItemService)
        {
            this.shoppingService = shoppingService;
            this.contentItemService = contentItemService;
        }


        public async Task<IActionResult> Index()
        {
            var cart = await shoppingService.GetCurrentShoppingCart();
            var images = await GetCartItemsImages(cart.Items.Select(x => x.VariantGraphQLId));
            var model = ShoppingCartContentViewModel.GetViewModel(cart, images);

            return View(model);
        }


        [HttpPost]
        [Route("/cart/update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([FromForm] string variantGraphQLId, [FromForm] int quantity, [FromForm] string cartOperation)
        {
            // TODO resolve country
            var country = ShopifySharp.GraphQL.CountryCode.CZ;
            var operationEnum = Enum.Parse<CartOperation>(cartOperation);

            if (operationEnum == CartOperation.Remove)
            {
                await shoppingService.RemoveCartItem(variantGraphQLId);
            }
            else
            {
                await shoppingService.UpdateCartItem(new ShoppingCartItemParameters()
                {
                    Country = country,
                    Quantity = quantity,
                    MerchandiseID = variantGraphQLId
                });
            }

            return Redirect(DancingGoatConstants.SHOPPING_CART_PATH);
        }

        [HttpPost]
        [Route("/cart/addDiscountCode")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> AddDiscountCode([FromForm] string discountCode)
        {
            await shoppingService.AddDiscountCode(discountCode);
            return Redirect(DancingGoatConstants.SHOPPING_CART_PATH);
        }

        [HttpPost]
        [Route("/cart/removeDiscountCode")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> RemoveDiscountCode([FromForm] string discountCode)
        {
            await shoppingService.RemoveDiscountCode(discountCode);
            return Redirect(DancingGoatConstants.SHOPPING_CART_PATH);
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
