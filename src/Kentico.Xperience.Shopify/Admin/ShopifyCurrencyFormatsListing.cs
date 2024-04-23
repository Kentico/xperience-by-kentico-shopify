using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Shopify.Admin;

[assembly: UIPage(
    parentType: typeof(ShopifyIntegrationSettingsApplication),
    slug: "shopify-currencies-formats-listing",
    uiPageType: typeof(ShopifyCurrencyFormatsListing),
    name: "Shopify currencies formats",
    templateName: TemplateNames.LISTING,
    order: 200)]
namespace Kentico.Xperience.Shopify.Admin
{
    public class ShopifyCurrencyFormatsListing : ListingPage
    {
        protected override string ObjectType => CurrencyFormatInfo.OBJECT_TYPE;

        public override Task ConfigurePage()
        {
            PageConfiguration.AddEditRowAction<ShopifyCurrencyFormatEdit>();
            PageConfiguration.HeaderActions.AddLink<ShopifyCurrencyFormatCreate>("New currency format");
            PageConfiguration.TableActions.AddDeleteAction(nameof(Delete));

            PageConfiguration.ColumnConfigurations
                .AddColumn(nameof(CurrencyFormatInfo.CurrencyCode), "Currency code")
                .AddColumn(nameof(CurrencyFormatInfo.CurrencyPriceFormat), "Currency price format");

            return base.ConfigurePage();
        }

        [PageCommand]
        public override Task<ICommandResponse<RowActionResult>> Delete(int id) => base.Delete(id);
    }
}
