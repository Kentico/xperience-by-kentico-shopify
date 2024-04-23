using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Shopify.Components.FormComponents;
using System.ComponentModel.DataAnnotations;

namespace DancingGoat.Components.Widgets.Shopify.ProductListWidget
{
    public class ShopifyProductListWidgetProperties : IWidgetProperties
    {

        [EditingComponent(TextInputComponent.IDENTIFIER, Label = "Title", Order = 0)]
        public string Title { get; set; }

        // TODO - create selector for long datatype
        [EditingComponent(ShopifyCollectionSelectorComponent.IDENTIFIER, Label = "Collection", Order = 10)]
        public string CollectionID { get; set; }

        [EditingComponent(ShopifyCurrencySelectorComponent.IDENTIFIER, Label = "Currency", Order = 20)]
        public string CurrencyCode { get; set; }

        [EditingComponent(IntInputComponent.IDENTIFIER, Label = "Max results", Order = 30)]
        [Range(1, 250)]
        public int Limit { get; set; }
    }
}
