using System.Globalization;

using CMS.Core;

using Kentico.Xperience.Shopify.Config;

using ShopifySharp.GraphQL;

namespace Kentico.Xperience.Shopify
{
    /// <summary>
    /// Provides extension methods for formatting decimal values as prices.
    /// </summary>
    public static class DecimalExtensions
    {
        private static readonly IReadOnlyDictionary<string, string> currencyFormats = Service.Resolve<IShopifyCurrencyFormatService>().GetFormats();


        /// <summary>
        /// Formats the specified price using <see cref="Admin.CurrencyFormatInfo"/> formats with the given currency code.
        /// </summary>
        /// <param name="price">The price to format.</param>
        /// <param name="currencyCode">The currency code to use for formatting.</param>
        /// <returns>The formatted price as a string.</returns>
        public static string FormatPrice(this decimal? price, string? currencyCode)
        {
            if (!price.HasValue || string.IsNullOrWhiteSpace(currencyCode))
            {
                return string.Empty;
            }

            return FormatPrice(price.Value, currencyCode);
        }


        /// <summary>
        /// Formats the specified price using <see cref="Admin.CurrencyFormatInfo"/> formats with the given currency code.
        /// </summary>
        /// <param name="price">The price to format.</param>
        /// <param name="currencyCode">The currency code to use for formatting.</param>
        /// <returns>The formatted price as a string.</returns>
        public static string FormatPrice(this decimal price, string currencyCode)
        {
            if (currencyFormats.TryGetValue(currencyCode, out string? format) &&
                !string.IsNullOrWhiteSpace(format))
            {
                return price.ToString(format, CultureInfo.InvariantCulture);
            }

            return price.ToString();
        }


        /// <summary>
        /// Formats the specified price using <see cref="Admin.CurrencyFormatInfo"/> formats with the given currency code.
        /// </summary>
        /// <param name="price">The price to format.</param>
        /// <param name="currencyCode">The currency code to use for formatting.</param>
        /// <returns>The formatted price as a string.</returns>
        public static string FormatPrice(this decimal price, CurrencyCode currencyCode) => FormatPrice(price, currencyCode.ToString());


        /// <summary>
        /// Formats the specified price using <see cref="Admin.CurrencyFormatInfo"/> formats with the given currency code.
        /// </summary>
        /// <param name="price">The price to format.</param>
        /// <param name="currencyCode">The currency code to use for formatting.</param>
        /// <returns>The formatted price as a string.</returns>
        public static string FormatPrice(this decimal? price, CurrencyCode currencyCode) => FormatPrice(price, currencyCode.ToString());
    }
}
