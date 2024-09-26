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
        /// <param name="orderId">Order identifier.</param>
        /// <returns>Retrieved Shopify order if found.</returns>
        Task<Order?> GetOrder(long orderId);
    }
}
