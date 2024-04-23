using Kentico.Xperience.Shopify.Admin;

namespace Kentico.Xperience.Shopify.Config
{
    /// <summary>
    /// Service for getting admin Shopify settings.
    /// </summary>
    public interface IShopifyIntegrationSettingsService
    {
        IntegrationSettingsInfo? GetSettings();
    }
}
