﻿using DancingGoat;
using DancingGoat.Controllers.Shopify;
using DancingGoat.Models;
using DancingGoat.Models.WebPage.Shopify.ShoppingCartPage;
using GreenDonut;
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
            ShoppingCartContentViewModel model = null;
            string[] errorMessages = TempData["ErrorMessages"] as string[] ?? [];

            if (cart == null)
            {
                model = new ShoppingCartContentViewModel()
                {
                    AppliedCoupons = [],
                    CartItems = [],
                    GrandTotal = string.Empty,
                    ErrorMessages = []
                };
            }
            else
            {
                var images = await GetCartItemsImages(cart.Items.Select(x => x.VariantGraphQLId));
                model = ShoppingCartContentViewModel.GetViewModel(cart, images, errorMessages);
            }

            return View(model);
        }


        [HttpPost]
        [Route("/cart/update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([FromForm] string variantGraphQLId, [FromForm] int quantity, [FromForm] string cartOperation)
        {
            var country = ShopifySharp.GraphQL.CountryCode.CZ;
            var operationEnum = Enum.Parse<CartOperation>(cartOperation);

            var result = operationEnum == CartOperation.Remove
                ? await shoppingService.RemoveCartItem(variantGraphQLId)
                : await shoppingService.UpdateCartItem(new ShoppingCartItemParameters()
                {
                    Country = country,
                    Quantity = quantity,
                    MerchandiseID = variantGraphQLId
                });

            AddErrorsToTempData(result);
            return Redirect(DancingGoatConstants.SHOPPING_CART_PATH);
        }

        [HttpPost]
        [Route("/cart/addDiscountCode")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> AddDiscountCode([FromForm] string discountCode)
        {
            var result = await shoppingService.AddDiscountCode(discountCode);

            AddErrorsToTempData(result);
            return Redirect(DancingGoatConstants.SHOPPING_CART_PATH);
        }

        [HttpPost]
        [Route("/cart/removeDiscountCode")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> RemoveDiscountCode([FromForm] string discountCode)
        {
            var result = await shoppingService.RemoveDiscountCode(discountCode);

            AddErrorsToTempData(result);
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


        private void AddErrorsToTempData(CartOperationResult result)
        {
            if (!result.Success)
            {
                TempData["ErrorMessages"] = result.ErrorMessages.ToArray();
            }
        }
    }
}
