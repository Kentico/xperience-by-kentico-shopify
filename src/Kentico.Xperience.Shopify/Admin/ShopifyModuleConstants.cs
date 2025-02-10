using Shopify;

namespace Kentico.Xperience.Shopify.Admin;

internal static class ShopifyResourceConstants
{
    public const string ResourceDisplayName = "Shopify configuration";
    public const string ResourceName = "ShopifyStoreConfiguration";
    public const string ResourceDescription = "The module integrates connection with Shopify API.";
    public const bool ResourceIsInDevelopment = false;

}

internal static class ShopifySettingsConstants
{
    public const string SettingsShopifyUrl = "Shopify store URL";
    public const string SettingsAdminApiKey = "Admin API key";
    public const string SettingsStorefrontApiKey = "Storefront API key";
    public const string SettingsStorefrontApiVersion = "Storefront API version";
    public const string SettingsStorefrontApiVersionExplanation = "Api version in format YYYY-MM. Admin API version is not needed since it is set by ShopifySharp NuGet package version";
    public const string SettingsProductSKUFolderGuid = "Content item folder for '" + Product.CONTENT_TYPE_NAME + "'";
    public const string SettingsProductVariantFolderGuid = "Content item folder for '" + ProductVariant.CONTENT_TYPE_NAME + "'";
    public const string SettingsProductImageFolderGuid = "Content item folder for '" + Image.CONTENT_TYPE_NAME + "'";
    public const string SettingsWorkspaceName = "Content item workspace name";
}
