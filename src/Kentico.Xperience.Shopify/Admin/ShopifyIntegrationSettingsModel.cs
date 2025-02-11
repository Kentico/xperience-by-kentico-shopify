using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace Kentico.Xperience.Shopify.Admin
{
    /// <summary>
    /// Model for Shopify integration settings.
    /// </summary>
    public class ShopifyIntegrationSettingsModel
    {
        /// <summary>
        /// The Shopify store URL.
        /// </summary>
        [UrlValidationRule]
        [TextInputComponent(Label = ShopifySettingsConstants.SettingsShopifyUrl, Order = 1)]
        [RequiredValidationRule]
        public string ShopifyStoreUrl { get; set; } = string.Empty;


        /// <summary>
        /// The Admin API key.
        /// </summary>
        [PasswordComponent(Label = ShopifySettingsConstants.SettingsAdminApiKey, Order = 2, IgnorePasswordPolicy = true)]
        [RequiredValidationRule]
        public string AdminApiKey { get; set; } = string.Empty;


        /// <summary>
        /// The Storefront API key.
        /// </summary>
        [PasswordComponent(Label = ShopifySettingsConstants.SettingsStorefrontApiKey, Order = 3, IgnorePasswordPolicy = true)]
        [RequiredValidationRule]
        public string StorefrontApiKey { get; set; } = string.Empty;


        /// <summary>
        /// The Storefront API version.
        /// </summary>
        [TextInputComponent(Label = ShopifySettingsConstants.SettingsStorefrontApiVersion, Order = 4, ExplanationText = ShopifySettingsConstants.SettingsStorefrontApiVersionExplanation)]
        [RequiredValidationRule]
        public string StorefrontApiVersion { get; set; } = string.Empty;
    }
}
