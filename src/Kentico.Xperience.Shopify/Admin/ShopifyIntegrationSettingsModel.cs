using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace Kentico.Xperience.Shopify.Admin
{
    public class ShopifyIntegrationSettingsModel
    {
        [UrlValidationRule]
        [TextInputComponent(Label = "Shopify store URL", Order = 1)]
        [RequiredValidationRule]
        public string? ShopifyStoreUrl { get; set; }


        [PasswordComponent(Label = "Admin API key", Order = 2, IgnorePasswordPolicy = true)]
        [RequiredValidationRule]
        public string? AdminApiKey { get; set; }


        [PasswordComponent(Label = "Storefront API key", Order = 3, IgnorePasswordPolicy = true)]
        [RequiredValidationRule]
        public string? StorefrontApiKey { get; set; }


        [TextInputComponent(Label = "Storefront API version", Order = 4, ExplanationText = "Api version in format YYYY-MM. Admin API version is not needed since it is set by ShopifySharp NuGet package version")]
        [RequiredValidationRule]
        public string? StorefrontApiVersion { get; set; }
    }
}
