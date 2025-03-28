using CMS.DataEngine;
using CMS.Helpers;
using CMS.Websites.Routing;

using Kentico.Xperience.Shopify.Admin;

using Microsoft.Extensions.Options;

using ShopifySharp.GraphQL;

namespace Kentico.Xperience.Shopify.Config
{
    internal class ShopifyIntegrationSettingsService : IShopifyIntegrationSettingsService
    {
        private readonly IProgressiveCache cache;
        private readonly ShopifyConfig shopifyConfig;
        private readonly IInfoProvider<IntegrationSettingsInfo> integrationSettingsProvider;
        private readonly ShopifyWebsiteChannelConfigOptions websiteChannelConfig;
        private readonly IWebsiteChannelContext websiteChannelContext;

        public ShopifyIntegrationSettingsService(
            IProgressiveCache cache,
            IOptionsMonitor<ShopifyConfig> shopifyConfigMonitor,
            IInfoProvider<IntegrationSettingsInfo> integrationSettingsProvider,
            IOptionsMonitor<ShopifyWebsiteChannelConfigOptions> websiteChannelConfigMonitor,
            IWebsiteChannelContext websiteChannelContext)
        {
            this.cache = cache;
            this.integrationSettingsProvider = integrationSettingsProvider;
            this.websiteChannelContext = websiteChannelContext;
            shopifyConfig = shopifyConfigMonitor.CurrentValue;
            websiteChannelConfig = websiteChannelConfigMonitor.CurrentValue;
        }

        public ShopifyConfig? GetSettings()
        {
            if (ShopifyConfigIsFilled(shopifyConfig))
            {
                return shopifyConfig;
            }

            return cache.Load(cs => GetConfigFromSettings(),
                new CacheSettings(20, $"{nameof(ShopifyIntegrationSettingsService)}|{nameof(GetSettings)}")
                {
                    CacheDependency = CacheHelper.GetCacheDependency($"{IntegrationSettingsInfo.OBJECT_TYPE}|all")
                });

        }

        public ShopifyWebsiteChannelConfig? GetWebsiteChannelSettings()
        {
            if (websiteChannelConfig is null)
            {
                return null;
            }

            string? currentChannel = websiteChannelContext.WebsiteChannelName;
            if (string.IsNullOrEmpty(currentChannel))
            {
                return websiteChannelConfig.DefaultSetting;
            }
            return websiteChannelConfig.Settings?.Find(x => x.ChannelName == currentChannel) ?? websiteChannelConfig.DefaultSetting;
        }

        public CountryCode? CountryByCurrency(CurrencyCode currency)
        {
            if (websiteChannelConfig is null)
            {
                return null;
            }

            return websiteChannelConfig.Settings?.Find(x => x.CurrencyCode == currency)?.Country ?? websiteChannelConfig.DefaultSetting?.Country;
        }

        public bool AdminUISettingsUsed()
            => !ShopifyConfigIsFilled(shopifyConfig);

        private ShopifyConfig? GetConfigFromSettings()
        {
            var settingsInfo = integrationSettingsProvider.Get()
                    .TopN(1)
                    .FirstOrDefault();
            if (settingsInfo is null)
            {
                return null;
            }

            return new ShopifyConfig()
            {
                AdminApiKey = settingsInfo.AdminApiKey,
                ShopifyUrl = settingsInfo.ShopifyUrl,
                StorefrontApiKey = settingsInfo.AdminApiKey,
                StorefrontApiVersion = settingsInfo.StorefrontApiVersion,
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
