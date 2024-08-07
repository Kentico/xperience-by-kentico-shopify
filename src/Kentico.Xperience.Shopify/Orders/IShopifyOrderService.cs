using ShopifySharp;

namespace Kentico.Xperience.Shopify.Orders
{
    /// <summary>
    /// Service for managing orders from Shopify store.
    /// </summary>
    public interface IShopifyOrderService
    {
        /// <summary>
        /// Get order from shopify by order identifier.
        /// </summary>
        /// <param name="shopifyOrderId">Shopify order identifier.</param>
        /// <returns>Order retrieved from Shopify or NULL if order with given ID does not exist.</returns>
        Task<Order?> GetOrder(long shopifyOrderId);
    }
}
