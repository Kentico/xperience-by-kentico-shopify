using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.Forms;
using Kentico.Xperience.Shopify.Admin;

[assembly: UIPage(
    parentType: typeof(ShopifyCurrencyFormatsListing),
    slug: "create",
    uiPageType: typeof(ShopifyCurrencyFormatCreate),
    name: "Create new currency format",
    templateName: TemplateNames.EDIT,
    order: 300)]
namespace Kentico.Xperience.Shopify.Admin
{
    public class ShopifyCurrencyFormatCreate : CreatePage<CurrencyFormatInfo, ShopifyCurrencyFormatEditSection>
    {
        public ShopifyCurrencyFormatCreate(IFormComponentMapper formComponentMapper, IFormDataBinder formDataBinder, IPageUrlGenerator pageUrlGenerator)
            : base(formComponentMapper, formDataBinder, pageUrlGenerator)
        {
        }

        public override Task ConfigurePage()
        {
            PageConfiguration.UIFormName = CurrencyFormatInfo.UI_FORM_NAME;
            return base.ConfigurePage();
        }
    }
}
