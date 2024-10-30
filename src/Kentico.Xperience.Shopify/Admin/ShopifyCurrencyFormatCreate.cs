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
    /// <summary>
    /// Page for creating a new currency format.
    /// </summary>
    public class ShopifyCurrencyFormatCreate : CreatePage<CurrencyFormatInfo, ShopifyCurrencyFormatEditSection>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShopifyCurrencyFormatCreate"/> class.
        /// </summary>
        /// <param name="formComponentMapper">The form component mapper.</param>
        /// <param name="formDataBinder">The form data binder.</param>
        /// <param name="pageLinkGenerator">The page link generator.</param>
        public ShopifyCurrencyFormatCreate(IFormComponentMapper formComponentMapper, IFormDataBinder formDataBinder, IPageLinkGenerator pageLinkGenerator)
            : base(formComponentMapper, formDataBinder, pageLinkGenerator)
        {
        }


        /// <inheritdoc/>
        public override Task ConfigurePage()
        {
            PageConfiguration.UIFormName = CurrencyFormatInfo.UI_FORM_NAME;
            return base.ConfigurePage();
        }
    }
}
