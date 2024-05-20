using ShopifySharp.GraphQL;

namespace Kentico.Xperience.Shopify.ShoppingCart
{
    /// <summary>
    /// Parameters of the cart item for shopping cart operations.
    /// </summary>
    public class ShoppingCartItemParameters
    {
        /// <summary>
        /// Quantity of the item.
        /// </summary>
        public int Quantity { get; set; }


        /// <summary>
        /// ID of the merchandise.
        /// </summary>
        public string MerchandiseID { get; set; } = string.Empty;


        /// <summary>
        /// Country code for the item.
        /// </summary>
        public CountryCode Country { get; set; }
    }
}
