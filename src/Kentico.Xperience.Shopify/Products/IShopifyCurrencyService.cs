namespace Kentico.Xperience.Shopify.Products
{
    /// <summary>
    /// Service for managing currencies.
    /// </summary>
    public interface IShopifyCurrencyService
    {
        /// <summary>
        /// Get available currency codes from Shopify store.
        /// </summary>
        /// <returns>ISO 4217 currency codes available in Shopify store.</returns>
        Task<IEnumerable<string>> GetCurrencyCodes();
    }
}
