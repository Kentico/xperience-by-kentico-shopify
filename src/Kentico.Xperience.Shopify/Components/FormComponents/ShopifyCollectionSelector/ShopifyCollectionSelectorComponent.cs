using Kentico.Forms.Web.Mvc;
using Kentico.Web.Mvc;
using Kentico.Xperience.Shopify.Components.FormComponents;
using Kentico.Xperience.Shopify.Services;

[assembly: RegisterFormComponent(ShopifyCollectionSelectorComponent.IDENTIFIER, typeof(ShopifyCollectionSelectorComponent), "Shopify collections dropdown", IconClass = "icon-menu")]
namespace Kentico.Xperience.Shopify.Components.FormComponents;
public class ShopifyCollectionSelectorComponent : SelectorFormComponent<ShopifyCollectionSelectorProperties>
{
    private readonly IShopifyCollectionService collectionService;

    public const string IDENTIFIER = "Kentico.Xperience.Shopify.ShopifyCollectionSelector";

    public ShopifyCollectionSelectorComponent(IShopifyCollectionService collectionService)
    {
        this.collectionService = collectionService;
    }

    // Retrieves data to be displayed in the selector
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
