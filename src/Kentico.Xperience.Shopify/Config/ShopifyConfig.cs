﻿namespace Kentico.Xperience.Shopify.Config
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

        /// <summary>
        /// Workspace name for synchronized content items.
        /// </summary>
        public required string WorkspaceName { get; set; }

        /// <summary>
        /// Target folder of synchronization of products
        /// </summary>
        public required Guid ProductSKUFolderGuid { get; set; }

        /// <summary>
        /// Target folder of synchronization of product variants
        /// </summary>
        public required Guid ProductVariantFolderGuid { get; set; }

        /// <summary>
        /// Target folder of synchronization of product images
        /// </summary>
        public required Guid ProductImageFolderGuid { get; set; }
    }
}
