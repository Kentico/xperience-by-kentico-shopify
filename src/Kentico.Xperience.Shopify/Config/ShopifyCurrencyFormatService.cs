using CMS.DataEngine;
using CMS.Helpers;

using Kentico.Xperience.Shopify.Admin;

namespace Kentico.Xperience.Shopify.Config
{
    internal class ShopifyCurrencyFormatService : IShopifyCurrencyFormatService
    {
        private readonly IProgressiveCache cache;
        private readonly IInfoProvider<CurrencyFormatInfo> currencyFormatInfoProvider;


        public ShopifyCurrencyFormatService(IProgressiveCache cache, IInfoProvider<CurrencyFormatInfo> currencyFormatInfoProvider)
        {
            this.cache = cache;
            this.currencyFormatInfoProvider = currencyFormatInfoProvider;
        }

        public IReadOnlyDictionary<string, string> GetFormats() =>
            cache.Load(cs => currencyFormatInfoProvider.Get()
                .ToDictionary(format => format.CurrencyCode, format => format.CurrencyPriceFormat)
            , new CacheSettings(20, $"{nameof(ShopifyCurrencyFormatService)}|{nameof(GetFormats)}")
            {
                CacheDependency = CacheHelper.GetCacheDependency($"{CurrencyFormatInfo.OBJECT_TYPE}|all")
            });
    }
}
