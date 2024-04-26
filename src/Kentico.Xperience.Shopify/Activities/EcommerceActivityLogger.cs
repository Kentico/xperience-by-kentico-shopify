using CMS.Activities;
using Kentico.Xperience.Shopify.ShoppingCart;
using Kentico.Xperience.Shopify.Synchronization;
using Shopify.ContentTypes;
using ShopifySharp.GraphQL;

namespace Kentico.Xperience.Shopify.Activities
{
    internal class EcommerceActivityLogger : IEcommerceActivityLogger
    {
        private readonly ICustomActivityLogger customActivityLogger;
        private readonly IShopifyContentItemService contentItemService;

        public EcommerceActivityLogger(
            ICustomActivityLogger customActivityLogger,
            IShopifyContentItemService shopifyContentItemService)
        {
            this.customActivityLogger = customActivityLogger;
            contentItemService = shopifyContentItemService;
        }


        public void LogProductAddedToShoppingCartActivity(ShoppingCartItem cartItem, int quantity)
        {
            customActivityLogger.Log(EcommerceActivityTypes.ProductAddedToCartActivity, new CustomActivityData()
            {
                ActivityTitle = $"Product added to shopping cart '{cartItem.Name}'",
                ActivityValue = quantity.ToString()
            });
        }


        public void LogProductRemovedFromShoppingCartActivity(ShoppingCartItem cartItem, int quantity)
        {
            customActivityLogger.Log(EcommerceActivityTypes.ProductAddedToCartActivity, new CustomActivityData()
            {
                ActivityTitle = $"Product removed from shopping cart '{cartItem.Name}'",
                ActivityValue = quantity.ToString()
            });
        }


        public void LogPurchaseActivity(decimal totalPrice, long orderId, CurrencyCode currency)
        {
            customActivityLogger.Log(EcommerceActivityTypes.ProductAddedToCartActivity, new CustomActivityData()
            {
                ActivityTitle = $"Purchase for {totalPrice.FormatPrice(currency)} (shopify orderID: {orderId})",
                ActivityValue = totalPrice.ToString(),
            });
        }


        public void LogPurchasedProductActivity(ShoppingCartItem cartItem, int quantity)
        {
            customActivityLogger.Log(EcommerceActivityTypes.PurchasedProductActivity, new CustomActivityData
            {
                ActivityTitle = $"Purchased product '{cartItem.Name}'",
                ActivityValue = quantity.ToString()
            });
        }


        //private async Task<(ShopifyProductVariantItem? productVariant, ShopifyProductItem? product)> GetShopifyContentItems(string variantGraphQLId)
        //{
        //    var shopifyVariantCI = (await contentItemService.GetVariants([variantGraphQLId])).FirstOrDefault();
        //    if (shopifyVariantCI == null)
        //    {
        //        return (null, null);
        //    }

        //    var shopifyProductCI = (await contentItemService.GetContentItems<ShopifyProductItem>(
        //            global::Shopify.Product.CONTENT_TYPE_NAME,
        //            (config) => config.Where(w => w.WhereEquals(nameof(ShopifyProductItem.ShopifyProductID), shopifyVariantCI.ShopifyProductID))
        //                    .TopN(1)
        //                    .WithLinkedItems(0))).FirstOrDefault();

        //    return (shopifyVariantCI, shopifyProductCI);
        //}
    }
}
