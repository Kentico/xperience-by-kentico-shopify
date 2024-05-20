using CMS.DataEngine;
using CMS.Helpers;

using Kentico.Xperience.Shopify.Admin;

using Microsoft.Extensions.Options;

namespace Kentico.Xperience.Shopify.Config
{
    internal class ShopifyIntegrationSettingsService : IShopifyIntegrationSettingsService
    {
        private readonly IProgressiveCache cache;
        private readonly IOptionsMonitor<ShopifyConfig> monitor;
        private readonly IInfoProvider<IntegrationSettingsInfo> integrationSettingsProvider;

        public ShopifyIntegrationSettingsService(
            IProgressiveCache cache,
            IOptionsMonitor<ShopifyConfig> monitor,
            IInfoProvider<IntegrationSettingsInfo> integrationSettingsProvider)
        {
            this.cache = cache;
            this.monitor = monitor;
            this.integrationSettingsProvider = integrationSettingsProvider;
        }

        public ShopifyConfig? GetSettings()
        {
            var monitorValue = monitor.CurrentValue;

            if (ShopifyConfigIsFilled(monitorValue))
            {
                return monitorValue;
            }

            return cache.Load(cs => GetConfigFromSettings(),
                new CacheSettings(20, $"{nameof(ShopifyIntegrationSettingsService)}|{nameof(GetSettings)}")
                {
                    CacheDependency = CacheHelper.GetCacheDependency($"{IntegrationSettingsInfo.OBJECT_TYPE}|all")
                });

        }

        private ShopifyConfig? GetConfigFromSettings()
        {
            var settingsInfo = integrationSettingsProvider.Get()
                    .TopN(1)
                    .FirstOrDefault();
            if (settingsInfo == null)
            {
                return null;
            }

            return new ShopifyConfig()
            {
                AdminApiKey = settingsInfo.AdminApiKey,
                ShopifyUrl = settingsInfo.ShopifyUrl,
                StorefrontApiKey = settingsInfo.AdminApiKey,
                StorefrontApiVersion = settingsInfo.StorefrontApiVersion
            };
        }

        private bool ShopifyConfigIsFilled(ShopifyConfig shopifyConfig)
            => shopifyConfig != null &&
               !string.IsNullOrEmpty(shopifyConfig.ShopifyUrl) &&
               !string.IsNullOrEmpty(shopifyConfig.AdminApiKey) &&
               !string.IsNullOrEmpty(shopifyConfig.StorefrontApiKey) &&
               !string.IsNullOrEmpty(shopifyConfig.StorefrontApiVersion);
    }
}
