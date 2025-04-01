using CMS.Activities;

using Kentico.Xperience.Shopify.ShoppingCart;

namespace Kentico.Xperience.Shopify.Activities
{
    internal class EcommerceActivityLogger : IEcommerceActivityLogger
    {
        private readonly ICustomActivityLogger customActivityLogger;

        public EcommerceActivityLogger(
            ICustomActivityLogger customActivityLogger)
        {
            this.customActivityLogger = customActivityLogger;
        }


        public void LogProductAddedToShoppingCartActivity(ShoppingCartItem? cartItem, int quantity)
        {
            customActivityLogger.Log(EcommerceActivityTypes.ProductAddedToCartActivity, new CustomActivityData()
            {
                ActivityTitle = $"Product added to shopping cart '{cartItem?.Name}'",
                ActivityValue = quantity.ToString()
            });
        }


        public void LogProductRemovedFromShoppingCartActivity(ShoppingCartItem? cartItem, int quantity)
        {
            customActivityLogger.Log(EcommerceActivityTypes.ProductRemovedFromCartActivity, new CustomActivityData()
            {
                ActivityTitle = $"Product removed from shopping cart '{cartItem?.Name}'",
                ActivityValue = quantity.ToString()
            });
        }


        public void LogPurchaseActivity(decimal totalPrice, string orderId, string currencyCode)
        {
            customActivityLogger.Log(EcommerceActivityTypes.PurchaseActivity, new CustomActivityData()
            {
                ActivityTitle = $"Purchase for {totalPrice.FormatPrice(currencyCode)} (shopify orderID: {orderId})",
                ActivityValue = totalPrice.ToString(),
            });
        }


        public void LogPurchasedProductActivity(ShoppingCartItem? cartItem)
        {
            customActivityLogger.Log(EcommerceActivityTypes.PurchasedProductActivity, new CustomActivityData
            {
                ActivityTitle = $"Purchased product '{cartItem?.Name}'",
                ActivityValue = cartItem?.Quantity.ToString()
            });
        }
    }
}
