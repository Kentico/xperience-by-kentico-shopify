using Kentico.Xperience.Shopify.Config;
using Kentico.Xperience.Shopify.Products;
using ShopifySharp;
using ShopifySharp.Factories;
using ShopifySharp.Filters;

namespace Kentico.Xperience.Shopify.Orders
{
    internal class ShopifyOrderService : ShopifyServiceBase, IShopifyOrderService
    {
        private const string ORDERS_FIELDS = "customer,source_identifier,name,order_status_url,id,line_items,total_price_set,presentment_currency";

        private readonly IOrderService orderService;

        public ShopifyOrderService(
            IOrderServiceFactory orderServiceFactory,
            IShopifyIntegrationSettingsService integrationSettingsService) : base(integrationSettingsService)
        {
            orderService = orderServiceFactory.Create(shopifyCredentials);
        }


        /// <inheritdoc/>
        public async Task<Order?> GetRecentOrder(string sourceId)
        {
            if (string.IsNullOrEmpty(sourceId))
            {
                return null;
            }

            var filter = new OrderListFilter()
            {
                CreatedAtMin = DateTime.Now.AddDays(-1).Date,
                Fields = ORDERS_FIELDS
            };

            var result = await orderService.ListAsync(filter);

            return result.Items.FirstOrDefault(x => x.SourceIdentifier == sourceId);
        }
    }
}
