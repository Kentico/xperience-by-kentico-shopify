namespace Kentico.Xperience.Shopify.Services
{
    public interface IShopifyCurrencyService
    {
        Task<IEnumerable<string>> GetCurrencyCodes();
    }
}