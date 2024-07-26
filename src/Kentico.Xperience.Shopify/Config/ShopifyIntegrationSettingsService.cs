using System.Diagnostics;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.Websites.Routing;

using Kentico.Xperience.Shopify.Admin;

using Microsoft.Extensions.Options;

namespace Kentico.Xperience.Shopify.Config
{
    internal class ShopifyIntegrationSettingsService : IShopifyIntegrationSettingsService
    {
        private readonly IProgressiveCache cache;
        private readonly IOptionsMonitor<ShopifyConfig> shopifyConfigMonitor;
        private readonly IInfoProvider<IntegrationSettingsInfo> integrationSettingsProvider;
        private readonly IOptionsMonitor<ShopifyWebsiteChannelConfigOptions> websiteChannelConfigMonitor;
        private readonly IWebsiteChannelContext websiteChannelContext;

        public ShopifyIntegrationSettingsService(
            IProgressiveCache cache,
            IOptionsMonitor<ShopifyConfig> shopifyConfigMonitor,
            IInfoProvider<IntegrationSettingsInfo> integrationSettingsProvider,
            IOptionsMonitor<ShopifyWebsiteChannelConfigOptions> websiteChannelConfigMonitor,
            IWebsiteChannelContext websiteChannelContext)
        {
            this.cache = cache;
            this.shopifyConfigMonitor = shopifyConfigMonitor;
            this.integrationSettingsProvider = integrationSettingsProvider;
            this.websiteChannelConfigMonitor = websiteChannelConfigMonitor;
            this.websiteChannelContext = websiteChannelContext;
        }

        public ShopifyConfig? GetSettings()
        {
            var monitorValue = shopifyConfigMonitor.CurrentValue;

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

        public ShopifyWebsiteChannelConfig? GetWebsiteChannelSettings()
        {
            var monitorValue = websiteChannelConfigMonitor.CurrentValue;

            if (monitorValue == null)
            {
                return null;
            }

            string? currentChannel = websiteChannelContext.WebsiteChannelName;
            if (string.IsNullOrEmpty(currentChannel))
            {
                return monitorValue.DefaultSetting;
            }
            return monitorValue.Settings?.Find(x => x.ChannelName == currentChannel) ?? monitorValue.DefaultSetting;
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
