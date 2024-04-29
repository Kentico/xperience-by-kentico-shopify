using ShopifySharp;

namespace Kentico.Xperience.Shopify.Orders
{
    public interface IShopifyOrderService
    {
        /// <summary>
        /// Get order from shopify by order source identifier.
        /// </summary>
        /// <param name="sourceId">Order source identifier.</param>
        /// <remarks>WARNING: Method looks for orders no older than 1 day.</remarks>
        /// <returns>Retrieved Shopify order if found.</returns>
        Task<Order?> GetOrder(string sourceId);
    }
}
