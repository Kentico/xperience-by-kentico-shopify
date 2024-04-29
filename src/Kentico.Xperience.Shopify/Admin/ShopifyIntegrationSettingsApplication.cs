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
    [UIPermission(SystemPermissions.VIEW)]
    [UIPermission(SystemPermissions.UPDATE)]
    public class ShopifyIntegrationSettingsApplication : ApplicationPage
    {
        public const string IDENTIFIER = "Kentico.Xperience.Shopify.Common.IntegrationSettings";
    }
}
