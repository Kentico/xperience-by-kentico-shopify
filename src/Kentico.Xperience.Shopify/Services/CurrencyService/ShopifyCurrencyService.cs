using Kentico.Xperience.Shopify.Config;
using Microsoft.Extensions.Options;
using ShopifySharp;
using ShopifySharp.Factories;

namespace Kentico.Xperience.Shopify.Services
{
    internal class ShopifyCurrencyService : ShopifyServiceBase, IShopifyCurrencyService
    {
        private readonly IShopService shopService;
        public ShopifyCurrencyService(IOptionsMonitor<ShopifyConfig> options, IShopServiceFactory shopServiceFactory) : base(options)
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
