using Kentico.Forms.Web.Mvc;
using Kentico.Web.Mvc;
using Kentico.Xperience.Shopify.Components.FormComponents;
using Kentico.Xperience.Shopify.Products;

[assembly: RegisterFormComponent(ShopifyCollectionSelectorComponent.IDENTIFIER, typeof(ShopifyCollectionSelectorComponent), "Shopify collections dropdown", IconClass = "icon-menu")]
namespace Kentico.Xperience.Shopify.Components.FormComponents;

/// <summary>
/// Form component for selecting Shopify collections.
/// </summary>
public class ShopifyCollectionSelectorComponent : SelectorFormComponent<ShopifyCollectionSelectorProperties>
{
    private readonly IShopifyCollectionService collectionService;

    /// <summary>
    /// The identifier of the Shopify collection selector component.
    /// </summary>
    public const string IDENTIFIER = "Kentico.Xperience.Shopify.ShopifyCollectionSelector";


    /// <summary>
    /// Initializes a new instance of the <see cref="ShopifyCollectionSelectorComponent"/> class.
    /// </summary>
    /// <param name="collectionService">The service for interacting with Shopify collections.</param>
    public ShopifyCollectionSelectorComponent(IShopifyCollectionService collectionService)
    {
        this.collectionService = collectionService;
    }


    /// <inheritdoc />
    protected override IEnumerable<HtmlOptionItem> GetHtmlOptions()
    {
        var collections = collectionService.GetCollectionListing().GetAwaiter().GetResult();

        yield return new HtmlOptionItem()
        {
            Value = "0",
            Text = "all"
        };

        foreach (var collection in collections)
        {
            yield return new HtmlOptionItem()
            {
                Value = collection.CollectionID.ToString(),
                Text = collection.Name
            };
        }
    }
}
