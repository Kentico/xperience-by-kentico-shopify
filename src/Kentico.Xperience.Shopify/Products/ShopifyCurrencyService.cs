using Kentico.Xperience.Shopify.Config;
using Kentico.Xperience.Shopify.Products.Models;

using ShopifySharp;
using ShopifySharp.Factories;


namespace Kentico.Xperience.Shopify.Products
{
    internal class ShopifyCurrencyService : ShopifyServiceBase, IShopifyCurrencyService
    {
        private readonly IGraphService graphService;
        public ShopifyCurrencyService(IShopifyIntegrationSettingsService integrationSettingsService, IGraphServiceFactory graphServiceFactory)
            : base(integrationSettingsService)
        {
            graphService = graphServiceFactory.Create(shopifyCredentials);
        }

        public async Task<IEnumerable<string>> GetCurrencyCodes()
        {
            return await TryCatch(GetCurrencyCodesInternal, Enumerable.Empty<string>);
        }

        public async Task<IEnumerable<string>> GetCurrencyCodesInternal()
        {
            var request = new GraphRequest()
            {
                Query = "query Markets { markets(first: 250) { nodes { currencySettings { baseCurrency { currencyCode } } } } }"
            };

            var response = await graphService.PostAsync<MarketListingModel>(request);
            var markets = response.Data.Markets.nodes;

            if (markets is null)
            {
                return [];
            }

            return markets.Select(x => x.currencySettings?.baseCurrency?.currencyCode?.ToString() ?? string.Empty);
        }
    }
}
