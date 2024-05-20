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
    /// <summary>
    /// Page for editing a currency format.
    /// </summary>
    public class ShopifyCurrencyFormatEdit : InfoEditPage<CurrencyFormatInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShopifyCurrencyFormatEdit"/> class.
        /// </summary>
        /// <param name="formComponentMapper">The form component mapper.</param>
        /// <param name="formDataBinder">The form data binder.</param>
        public ShopifyCurrencyFormatEdit(IFormComponentMapper formComponentMapper, IFormDataBinder formDataBinder)
            : base(formComponentMapper, formDataBinder)
        {
        }


        /// <inheritdoc/>
        public override Task ConfigurePage()
        {
            PageConfiguration.UIFormName = CurrencyFormatInfo.UI_FORM_NAME;
            return base.ConfigurePage();
        }


        /// <inheritdoc/>
        [PageParameter(typeof(IntPageModelBinder))]
        public override int ObjectId { get; set; }
    }
}
