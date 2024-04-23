using CMS.Helpers;

using Kentico.Xperience.Shopify.Admin;

namespace Kentico.Xperience.Shopify.Config
{
    internal class ShopifyIntegrationSettingsService : IShopifyIntegrationSettingsService
    {
        private readonly IProgressiveCache cache;

        public ShopifyIntegrationSettingsService(IProgressiveCache cache)
        {
            this.cache = cache;
        }

        public IntegrationSettingsInfo? GetSettings() =>
            cache.Load(cs => IntegrationSettingsInfo.Provider.Get()
                .TopN(1)
                .FirstOrDefault()
            , new CacheSettings(20, $"{nameof(ShopifyIntegrationSettingsService)}|{nameof(GetSettings)}")
            {
                CacheDependency = CacheHelper.GetCacheDependency($"{IntegrationSettingsInfo.OBJECT_TYPE}|all")
            });
    }
}
