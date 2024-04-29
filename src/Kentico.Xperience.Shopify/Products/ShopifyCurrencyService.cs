using Kentico.Xperience.Shopify.Config;

using ShopifySharp;
using ShopifySharp.Factories;

namespace Kentico.Xperience.Shopify.Products
{
    internal class ShopifyCurrencyService : ShopifyServiceBase, IShopifyCurrencyService
    {
        private readonly IShopService shopService;
        public ShopifyCurrencyService(IShopifyIntegrationSettingsService integrationSettingsService, IShopServiceFactory shopServiceFactory)
            : base(integrationSettingsService)
        {
            shopService = shopServiceFactory.Create(shopifyCredentials);
        }

        public async Task<IEnumerable<string>> GetCurrencyCodes()
        {
            return await TryCatch(GetCurrencyCodesInternal, Enumerable.Empty<string>);
        }

        public async Task<IEnumerable<string>> GetCurrencyCodesInternal()
        {
            var shop = await shopService.GetAsync();
            return shop.EnabledPresentmentCurrencies;
        }
    }
}
