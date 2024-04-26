using Kentico.Xperience.Shopify.ShoppingCart;
using ShopifySharp.GraphQL;

namespace Kentico.Xperience.Shopify.Activities
{
    public interface IEcommerceActivityLogger
    {
        /// <summary>
        /// Logs activity product added to shopping cart. 
        /// </summary>
        /// <param name="cartItem">New shopping cart item that was added to the shopping cart.</param>
        /// <param name="quantity">Cart item quantity.</param>
        void LogProductAddedToShoppingCartActivity(ShoppingCartItem? cartItem, int quantity);


        /// <summary>
        /// Logs activity product removed to shopping cart. 
        /// </summary>
        /// <param name="cartItem">Shopping cart item that was removed from the shopping cart.</param>
        /// <param name="quantity">Removed cart item quantity.</param>
        void LogProductRemovedFromShoppingCartActivity(ShoppingCartItem? cartItem, int quantity);


        /// <summary>
        /// Logs purchase activity.
        /// </summary>
        /// <param name="totalPrice">Order total price in shopping cart currency.</param>
        /// <param name="orderId">Order ID.</param>
        /// <param name="currency">Shopping cart currency.</param>
        void LogPurchaseActivity(decimal totalPrice, long orderId, CurrencyCode currency);


        /// <summary>
        /// Logs product purchased activity.
        /// </summary>
        /// <param name="cartItem">Purchased shopping cart item.</param>
        /// <param name="quantity">Quantity.</param>
        void LogPurchasedProductActivity(ShoppingCartItem? cartItem, int quantity);
    }
}
