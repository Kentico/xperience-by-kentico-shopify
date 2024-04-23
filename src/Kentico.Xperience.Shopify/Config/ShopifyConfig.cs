namespace Kentico.Xperience.Shopify.Config
{
    public class ShopifyConfig
    {
        public readonly static string SECTION_NAME = "CMSShopifyConfig";

        /// <summary>
        /// Shopify store URL
        /// </summary>
        public required string ShopifyUrl { get; set; }

        /// <summary>
        /// Admin API token
        /// </summary>
        public required string AdminApiToken { get; set; }

        /// <summary>
        /// Storefront API token
        /// </summary>
        public required string StorefrontApiToken { get; set; }

        /// <summary>
        /// Storefront API version
        /// </summary>
        public required string StorefrontApiVersion { get; set; }

        /// <summary>
        /// Dictionary where key is currency code and value is the format
        /// </summary>
        public required IDictionary<string, string> CurrencyFormats { get; set; }
    }
}

