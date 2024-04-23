using CMS.Core;
using CMS.Helpers;

namespace Kentico.Xperience.Shopify.ShoppingCart
{
    internal class ShoppingCartCacheService : IShoppingCartCacheService
    {
        private const string CACHE_KEY_FORMAT = $"{nameof(ShoppingCartInfo)}|{{0}}";

        private readonly IConversionService conversionService;
        private readonly ISettingsService settingsService;
        private readonly IProgressiveCache progressiveCache;


        public ShoppingCartCacheService(IConversionService conversionService, ISettingsService settingsService, IProgressiveCache progressiveCache)
        {
            this.conversionService = conversionService;
            this.settingsService = settingsService;
            this.progressiveCache = progressiveCache;
        }

        private string CacheKey(string cartId) => string.Format(CACHE_KEY_FORMAT, cartId);


        public void UpdateCartCache(ShoppingCartInfo cart)
        {
            string cacheKey = CacheKey(cart.CartId);
            CacheHelper.Remove(cacheKey, false, false);
            CacheHelper.Add(cacheKey, cart, null, DateTimeOffset.Now.AddMinutes(10), TimeSpan.Zero);
        }


        public async Task<ShoppingCartInfo?> LoadAsync(string cartId, Func<string, Task<ShoppingCartInfo?>> retriveCartFunc)
        {
            int cacheMinutes = conversionService.GetInteger(settingsService["CMSCacheMinutes"], 0);
            return await progressiveCache.LoadAsync(
                async (_) => await retriveCartFunc(cartId),
                new CacheSettings(cacheMinutes, CacheKey(cartId)));
        }
    }
}
