namespace Kentico.Xperience.Shopify.Config
{
    /// <summary>
    /// Configuration settings for Shopify.
    /// </summary>
    public class ShopifyConfig
    {
        /// <summary>
        /// The name of the configuration section.
        /// </summary>
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
