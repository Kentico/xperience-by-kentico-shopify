namespace Kentico.Xperience.Shopify.Config
{
    /// <summary>
    /// Service for getting admin Shopify settings.
    /// </summary>
    public interface IShopifyIntegrationSettingsService
    {
        ShopifyConfig? GetSettings();
    }
}
