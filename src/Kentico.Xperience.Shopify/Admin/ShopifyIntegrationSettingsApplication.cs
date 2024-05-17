using CMS.Membership;

using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.UIPages;
using Kentico.Xperience.Shopify.Admin;

[assembly: UIApplication(
    identifier: ShopifyIntegrationSettingsApplication.IDENTIFIER,
    type: typeof(ShopifyIntegrationSettingsApplication),
    slug: "shopify-integration",
    name: "Shopify integration",
    category: BaseApplicationCategories.CONFIGURATION,
    icon: Icons.IntegrationScheme,
    templateName: TemplateNames.SECTION_LAYOUT
    )]

namespace Kentico.Xperience.Shopify.Admin
{
    /// <summary>
    /// Application for managing Shopify integration settings.
    /// </summary>
    [UIPermission(SystemPermissions.VIEW)]
    [UIPermission(SystemPermissions.UPDATE)]
    public class ShopifyIntegrationSettingsApplication : ApplicationPage
    {
        /// <summary>
        /// The identifier of the Shopify integration settings application.
        /// </summary>
        public const string IDENTIFIER = "Kentico.Xperience.Shopify.Common.IntegrationSettings";
    }
}
