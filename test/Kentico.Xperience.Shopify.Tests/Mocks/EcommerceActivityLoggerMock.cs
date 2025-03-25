using Kentico.Xperience.Shopify.Activities;
using Kentico.Xperience.Shopify.ShoppingCart;

namespace Kentico.Xperience.Shopify.Tests.Mocks
{
    internal class EcommerceActivityLoggerMock : IEcommerceActivityLogger
    {
        public void LogProductAddedToShoppingCartActivity(ShoppingCartItem? cartItem, int quantity)
        {
        }

        public void LogProductRemovedFromShoppingCartActivity(ShoppingCartItem? cartItem, int quantity)
        {
        }

        public void LogPurchaseActivity(decimal totalPrice, string orderId, string currencyCode)
        {
        }

        public void LogPurchasedProductActivity(ShoppingCartItem? cartItem)
        {
        }
    }
}
