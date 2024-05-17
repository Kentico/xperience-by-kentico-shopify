using Kentico.Forms.Web.Mvc;
using Kentico.Web.Mvc;
using Kentico.Xperience.Shopify.Components.FormComponents;
using Kentico.Xperience.Shopify.Products;

[assembly: RegisterFormComponent(ShopifyCurrencySelectorComponent.IDENTIFIER, typeof(ShopifyCurrencySelectorComponent), "Shopify enabled currencies dropdown", IconClass = "icon-dollar-sign")]
namespace Kentico.Xperience.Shopify.Components.FormComponents;

/// <summary>
/// Form component for selecting Shopify enabled currencies.
/// </summary>
public class ShopifyCurrencySelectorComponent : SelectorFormComponent<ShopifyCurrencySelectorProperties>
{
    private readonly IShopifyCurrencyService currencyService;

    /// <summary>
    /// The identifier of the Shopify currency selector component.
    /// </summary>
    public const string IDENTIFIER = "Kentico.Xperience.Shopify.ShopifyCurrencySelector";


    /// <summary>
    /// Initializes a new instance of the <see cref="ShopifyCurrencySelectorComponent"/> class.
    /// </summary>
    /// <param name="currencyService">The service for interacting with Shopify currencies.</param>
    public ShopifyCurrencySelectorComponent(IShopifyCurrencyService currencyService) : base()
    {
        this.currencyService = currencyService;
    }


    /// <inheritdoc/>
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
