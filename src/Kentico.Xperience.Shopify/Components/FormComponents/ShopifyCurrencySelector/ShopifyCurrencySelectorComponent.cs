using Kentico.Forms.Web.Mvc;
using Kentico.Web.Mvc;
using Kentico.Xperience.Shopify.Components.FormComponents;
using Kentico.Xperience.Shopify.Services;

[assembly: RegisterFormComponent(ShopifyCurrencySelectorComponent.IDENTIFIER, typeof(ShopifyCurrencySelectorComponent), "Shopify enabled currencies dropdown", IconClass = "icon-dollar-sign")]
namespace Kentico.Xperience.Shopify.Components.FormComponents;

public class ShopifyCurrencySelectorComponent : SelectorFormComponent<ShopifyCurrencySelectorProperties>
{
    private readonly IShopifyCurrencyService currencyService;

    public const string IDENTIFIER = "Kentico.Xperience.Shopify.ShopifyCurrencySelector";

    public ShopifyCurrencySelectorComponent(IShopifyCurrencyService currencyService) : base()
    {
        this.currencyService = currencyService;
    }

    protected override IEnumerable<HtmlOptionItem> GetHtmlOptions()
    {
        var currencies = currencyService.GetCurrencyCodes().GetAwaiter().GetResult();

        yield return new HtmlOptionItem()
        {
            Value = string.Empty,
            Text = "default"
        };

        foreach (string? currency in currencies)
        {
            yield return new HtmlOptionItem()
            {
                Value = currency,
                Text = currency
            };
        }
    }
}
