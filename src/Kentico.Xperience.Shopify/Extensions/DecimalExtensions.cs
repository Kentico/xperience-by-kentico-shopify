using System.Globalization;

using CMS.Core;

using Kentico.Xperience.Shopify.Config;

namespace Kentico.Xperience.Shopify
{
    public static class DecimalExtensions
    {
        private static readonly IReadOnlyDictionary<string, string> currencyFormats = Service.Resolve<IShopifyCurrencyFormatService>().GetFormats();

        public static string FormatPrice(this decimal? price, string? currencyCode)
        {
            if (!price.HasValue || string.IsNullOrWhiteSpace(currencyCode))
            {
                return string.Empty;
            }

            return FormatPrice(price.Value, currencyCode);
        }

        public static string FormatPrice(this decimal price, string currencyCode)
        {
            if (currencyFormats.TryGetValue(currencyCode, out string? format) &&
                !string.IsNullOrWhiteSpace(format))
            {
                return price.ToString(format, CultureInfo.InvariantCulture);
            }

            return price.ToString();
        }
    }
}
