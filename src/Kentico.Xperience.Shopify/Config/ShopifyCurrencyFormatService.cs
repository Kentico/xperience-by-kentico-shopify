using CMS.Helpers;

using Kentico.Xperience.Shopify.Admin;

namespace Kentico.Xperience.Shopify.Config
{
    internal class ShopifyCurrencyFormatService : IShopifyCurrencyFormatService
    {
        private readonly IProgressiveCache cache;

        public ShopifyCurrencyFormatService(IProgressiveCache cache)
        {
            this.cache = cache;
        }

        public IReadOnlyDictionary<string, string> GetFormats() =>
            cache.Load(cs => CurrencyFormatInfo.Provider.Get()
                .ToDictionary(format => format.CurrencyCode, format => format.CurrencyPriceFormat)
            , new CacheSettings(20, $"{nameof(ShopifyCurrencyFormatService)}|{nameof(GetFormats)}")
            {
                CacheDependency = CacheHelper.GetCacheDependency($"{CurrencyFormatInfo.OBJECT_TYPE}|all")
            });
    }
}
