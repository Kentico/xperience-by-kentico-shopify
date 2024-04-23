using ShopifySharp.GraphQL;

namespace Kentico.Xperience.Shopify.ShoppingCart
{
    public class ShoppingCartItemParameters
    {
        public int Quantity { get; set; }
        public string MerchandiseID { get; set; } = string.Empty;
        public CountryCode Country { get; set; }
    }
}
