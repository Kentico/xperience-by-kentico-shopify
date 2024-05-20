using Kentico.Xperience.Shopify.Products.Models;

namespace DancingGoat.Components.Widgets.Shopify.ProductListWidget
{
    public class ShopifyProductListViewModel
    {
        public string Title { get; set; }
        public IEnumerable<ShopifyProductListModel> Products { get; set; }
        public string Currency { get; set; }
    }
}