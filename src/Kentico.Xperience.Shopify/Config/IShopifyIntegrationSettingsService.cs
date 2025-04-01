using ShopifySharp.GraphQL;

namespace Kentico.Xperience.Shopify.Config
{
    /// <summary>
    /// Service for getting admin Shopify settings.
    /// </summary>
    public interface IShopifyIntegrationSettingsService
    {
        /// <summary>
        /// Get shopify configuration form appsettings or database if
        /// no appsettings does not contain these settings.
        /// </summary>
        /// <returns><see cref="ShopifyConfig"/> containing the settings or NULL if no configuration is found.</returns>
        ShopifyConfig? GetSettings();

        /// <summary>
        /// Get current website channel configuration from appsettings.
        /// </summary>
        /// <returns>
        /// <see cref="ShopifyWebsiteChannelConfig"/> containing configuration for current website channel or default value if no configuration is found.
        /// </returns>
        ShopifyWebsiteChannelConfig? GetWebsiteChannelSettings();

        /// <summary>
        /// Check if settings from admin UI are being used. If not, values from appsettings.json or
        /// user secrets are used.
        /// </summary>
        /// <returns>True if settings from admin UI are used. Otherwise False.</returns>
        bool AdminUISettingsUsed();

        /// <summary>
        /// Get country code by currency code based on data from appsettings.json or user secrets.
        /// </summary>
        /// <param name="currency"></param>
        /// <returns>Country code based on currency code.</returns>
        CountryCode? CountryByCurrency(CurrencyCode currency);
    }
}
