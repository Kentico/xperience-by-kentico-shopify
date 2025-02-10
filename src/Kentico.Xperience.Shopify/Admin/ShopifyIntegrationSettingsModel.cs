using CMS.Workspaces.Internal;

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

        /// <summary>
        /// Workspace name
        /// </summary>
        [RequiredValidationRule]
        [DropDownComponent(Label = ShopifySettingsConstants.SettingsWorkspaceName, DataProviderType = typeof(WorkspaceOptionsProvider), Order = 0)]
        public string WorkspaceName { get; set; } = WorkspaceConstants.WORKSPACE_DEFAULT_CODE_NAME;

        /// <summary>
        /// Product content item folder ID.
        /// </summary>
        [RequiredValidationRule]
        //[DropDownComponent(Label = K13EcommerceSettingsConstants.SettingsProductSKUFolderGuid, DataProviderType = typeof(ContentFolderOptionsProvider), Order = 10)]
        [ContentFolderSelectorComponent(Label = ShopifySettingsConstants.SettingsProductSKUFolderGuid, Order = 10)]
        [FormComponentConfiguration(ContentFolderWorkspaceConfigurator.IDENTIFIER, nameof(WorkspaceName))]
        public int ProductSKUFolderId { get; set; }

        /// <summary>
        /// Product variant content item folder ID.
        /// </summary>
        [RequiredValidationRule]
        //[DropDownComponent(Label = K13EcommerceSettingsConstants.SettingsProductVariantFolderGuid, DataProviderType = typeof(ContentFolderOptionsProvider), Order = 20)]
        [ContentFolderSelectorComponent(Label = ShopifySettingsConstants.SettingsProductVariantFolderGuid, Order = 20)]
        [FormComponentConfiguration(ContentFolderWorkspaceConfigurator.IDENTIFIER, nameof(WorkspaceName))]
        public int ProductVariantFolderId { get; set; }

        /// <summary>
        /// Product image content item folder ID.
        /// </summary>
        [RequiredValidationRule]
        //[DropDownComponent(Label = K13EcommerceSettingsConstants.SettingsProductImageFolderGuid, DataProviderType = typeof(ContentFolderOptionsProvider), Order = 30)]
        [ContentFolderSelectorComponent(Label = ShopifySettingsConstants.SettingsProductImageFolderGuid, Order = 30)]
        [FormComponentConfiguration(ContentFolderWorkspaceConfigurator.IDENTIFIER, nameof(WorkspaceName))]
        public int ProductImageFolderId { get; set; }
    }
}
