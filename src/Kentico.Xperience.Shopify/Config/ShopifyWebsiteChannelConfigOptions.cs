namespace Kentico.Xperience.Shopify.Config
{
    /// <summary>
    /// Class with list of website channel configurations.
    /// </summary>
    public class ShopifyWebsiteChannelConfigOptions
    {
        /// <summary>
        /// The name of configuration section.
        /// </summary>
        public const string SECTION_NAME = "CMSShopifyWebsiteChannelsConfig";

        /// <summary>
        /// Website channel configurations list.
        /// </summary>
        public required List<ShopifySpecificWebsiteChannelConfig> Settings { get; set; }

        /// <summary>
        /// Default setting used if no setting for current website channel is found.
        /// </summary>
        public required ShopifyWebsiteChannelConfig? DefaultSetting { get; set; }
    }


    /// <summary>
    /// Class for website channel configuration
    /// </summary>
    public class ShopifySpecificWebsiteChannelConfig : ShopifyWebsiteChannelConfig
    {
        /// <summary>
        /// Website channel name.
        /// </summary>
        public required string ChannelName { get; set; }
    }


    /// <summary>
    /// Class for default channel configuration.
    /// </summary>
    public class ShopifyWebsiteChannelConfig
    {
        /// <summary>
        /// ISO 4217 currency code.
        /// </summary>
        public required string CurrencyCode { get; set; }

        /// <summary>
        /// Two letter country code.
        /// </summary>
        public required string Country { get; set; }
    }
}
