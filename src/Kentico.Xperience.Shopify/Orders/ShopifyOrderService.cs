using CMS.Helpers;

using Kentico.Xperience.Shopify.Config;
using Kentico.Xperience.Shopify.Orders.Models;
using Kentico.Xperience.Shopify.Products;

using ShopifySharp;
using ShopifySharp.Factories;

namespace Kentico.Xperience.Shopify.Orders
{
    internal class ShopifyOrderService : ShopifyServiceBase, IShopifyOrderService
    {
        private readonly IGraphService graphService;

        public ShopifyOrderService(
            IGraphServiceFactory graphServiceFactory,
            IShopifyIntegrationSettingsService integrationSettingsService) : base(integrationSettingsService)
        {
            graphService = graphServiceFactory.Create(shopifyCredentials);
        }


        /// <inheritdoc/>
        public async Task<OrderModel?> GetOrder(long orderId)
        {
            if (orderId == 0)
            {
                return null;
            }

            var request = new GraphRequest()
            {
                Query = "query getOrder($orderId:ID!){order(id:$orderId){id statusPageUrl name customer{firstName lastName email defaultAddress{address1 city zip countryCodeV2}}totalPriceSet{presentmentMoney{amount currencyCode}}}}",
                Variables = new Dictionary<string, object>()
                {
                    ["orderId"] = $"gid://shopify/Order/{orderId}"
                }
            };

            var order = await TryCatch(
                async () => (await graphService.PostAsync<OrderQueryResult>(request)).Data.Order,
                () => null);

            if (order?.customer is null)
            {
                return null;
            }

            return new OrderModel()
            {
                Id = order.id ?? string.Empty,
                Name = order.name ?? string.Empty,
                StatusPageUrl = order.statusPageUrl ?? string.Empty,
                FirstName = order.customer.firstName ?? string.Empty,
                LastName = order.customer.lastName ?? string.Empty,
                Email = order.customer.email ?? string.Empty,
                Amount = order.totalPriceSet?.presentmentMoney?.amount ?? 0,
                CurrencyCode = order.totalPriceSet?.presentmentMoney?.currencyCode.ToStringRepresentation() ?? string.Empty,
                DefaultAddress = new OrderAddressModel()
                {
                    Address1 = order.customer.defaultAddress?.address1 ?? string.Empty,
                    City = order.customer.defaultAddress?.city ?? string.Empty,
                    Zip = order.customer.defaultAddress?.zip ?? string.Empty,
                    CountryCode = order.customer.defaultAddress?.countryCodeV2.ToStringRepresentation() ?? string.Empty
                }
            };
        }
    }
}
