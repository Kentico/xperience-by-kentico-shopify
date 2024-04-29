using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Shopify.Admin;

[assembly: UIPage(
    parentType: typeof(ShopifyCurrencyFormatsListing),
    slug: PageParameterConstants.PARAMETERIZED_SLUG,
    uiPageType: typeof(ShopifyCurrencyFormatEditSection),
    name: "Edit section for currency format objects",
    templateName: TemplateNames.SECTION_LAYOUT,
    order: 400)]
namespace Kentico.Xperience.Shopify.Admin
{
    public class ShopifyCurrencyFormatEditSection : EditSectionPage<CurrencyFormatInfo>
    {
    }
}
