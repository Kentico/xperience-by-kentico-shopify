using CMS.DataEngine;
using CMS.Helpers;

using Kentico.Xperience.Shopify.Admin;

namespace Kentico.Xperience.Shopify.Config
{
    internal class ShopifyCurrencyFormatService : IShopifyCurrencyFormatService
    {
        private readonly IProgressiveCache cache;
        private readonly IInfoProvider<CurrencyFormatInfo> currencyFormatProvider;


        public ShopifyCurrencyFormatService(IProgressiveCache cache, IInfoProvider<CurrencyFormatInfo> currencyFormatProvider)
        {
            this.cache = cache;
            this.currencyFormatProvider = currencyFormatProvider;
        }

        public IReadOnlyDictionary<string, string> GetFormats() =>
            cache.Load(cs => currencyFormatProvider.Get()
                .ToDictionary(format => format.CurrencyCode, format => format.CurrencyPriceFormat)
            , new CacheSettings(20, $"{nameof(ShopifyCurrencyFormatService)}|{nameof(GetFormats)}")
            {
                CacheDependency = CacheHelper.GetCacheDependency($"{CurrencyFormatInfo.OBJECT_TYPE}|all")
            });
    }
}
