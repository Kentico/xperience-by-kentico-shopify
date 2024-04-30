namespace Kentico.Xperience.Shopify.Config
{
    public class ShopifyConfig
    {
        public static readonly string SECTION_NAME = "CMSShopifyConfig";

        /// <summary>
        /// Shopify store URL
        /// </summary>
        public required string ShopifyUrl { get; set; }

        /// <summary>
        /// Admin API token
        /// </summary>
        public required string AdminApiKey { get; set; }

        /// <summary>
        /// Storefront API token
        /// </summary>
        public required string StorefrontApiKey { get; set; }

        /// <summary>
        /// Storefront API version
        /// </summary>
        public required string StorefrontApiVersion { get; set; }
    }
}
