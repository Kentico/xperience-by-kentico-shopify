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
    }
}
