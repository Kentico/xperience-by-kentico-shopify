using Kentico.Xperience.Shopify.Config;
using Kentico.Xperience.Shopify.Products;
using ShopifySharp;
using ShopifySharp.Factories;
using ShopifySharp.Filters;

namespace Kentico.Xperience.Shopify.Orders
{
    internal class ShopifyOrderService : ShopifyServiceBase, IShopifyOrderService
    {
        private readonly IOrderService orderService;


        public ShopifyOrderService(IOrderServiceFactory orderServiceFactory, IShopifyIntegrationSettingsService integrationSettingsService) : base(integrationSettingsService)
        {
            orderService = orderServiceFactory.Create(shopifyCredentials);
        }

        public async Task<OrderCustomerDetails?> GetOrderCustomerDetails(string cartToken)
        {
            var filter = new OrderListFilter()
            {
                CreatedAtMin = DateTime.Now.AddMinutes(-10).Date,
                Fields = "customer"
            };

            var result = await orderService.ListAsync(filter);

            var customer = result.Items.FirstOrDefault(x => x.CartToken == cartToken)?.Customer;
            if (customer == null)
            {
                return null;
            }


            return new OrderCustomerDetails()
            {
                Email = customer.Email,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Address1 = customer.DefaultAddress.Address1,
                City = customer.DefaultAddress.City,
                Zip = customer.DefaultAddress.Zip,
                CountryCode = customer.DefaultAddress.CountryCode
            };
        }
    }
}
