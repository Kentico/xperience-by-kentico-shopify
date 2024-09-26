using Kentico.Xperience.Shopify.Config;
using Kentico.Xperience.Shopify.Products;

using ShopifySharp;
using ShopifySharp.Factories;

namespace Kentico.Xperience.Shopify.Orders
{
    internal class ShopifyOrderService : ShopifyServiceBase, IShopifyOrderService
    {
        private const string ORDER_FIELDS = "customer,source_identifier,name,order_status_url,id,line_items,total_price_set,presentment_currency";

        private readonly IOrderService orderService;

        public ShopifyOrderService(
            IOrderServiceFactory orderServiceFactory,
            IShopifyIntegrationSettingsService integrationSettingsService) : base(integrationSettingsService)
        {
            orderService = orderServiceFactory.Create(shopifyCredentials);
        }


        /// <inheritdoc/>
        public async Task<Order?> GetOrder(long orderId)
        {
            if (orderId == 0)
            {
                return null;
            }

            return await TryCatch<Order?>(
                async () => await orderService.GetAsync(orderId, ORDER_FIELDS),
                () => null);
        }
    }
}
