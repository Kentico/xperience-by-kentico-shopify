namespace Kentico.Xperience.Shopify.Config
{
    internal interface IShopifySynchronizationSettingsService
    {
        Task<SynchronizationConfig> GetSettings();
    }
}
