using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.Forms;
using Kentico.Xperience.Shopify.Admin;

[assembly: UIPage(
    parentType: typeof(ShopifyCurrencyFormatEditSection),
    slug: "edit",
    uiPageType: typeof(ShopifyCurrencyFormatEdit),
    name: "Edit currency format",
    templateName: TemplateNames.EDIT,
    order: UIPageOrder.First)]
namespace Kentico.Xperience.Shopify.Admin
{
    public class ShopifyCurrencyFormatEdit : InfoEditPage<CurrencyFormatInfo>
    {
        public ShopifyCurrencyFormatEdit(IFormComponentMapper formComponentMapper, IFormDataBinder formDataBinder)
            : base(formComponentMapper, formDataBinder)
        {
        }

        public override Task ConfigurePage()
        {
            PageConfiguration.UIFormName = CurrencyFormatInfo.UI_FORM_NAME;
            return base.ConfigurePage();
        }

        [PageParameter(typeof(IntPageModelBinder))]
        public override int ObjectId { get; set; }
    }
}
