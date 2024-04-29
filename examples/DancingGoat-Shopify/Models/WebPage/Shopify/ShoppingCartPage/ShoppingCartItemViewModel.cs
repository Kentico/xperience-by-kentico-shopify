namespace DancingGoat.Models.WebPage.Shopify.ShoppingCartPage
{
    public class ShoppingCartItemViewModel
    {
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
        public string VariantGraphQLId { get; set; }
        public string CartItemId { get; set; }
        public string ItemPrice { get; set; }
    }
}
