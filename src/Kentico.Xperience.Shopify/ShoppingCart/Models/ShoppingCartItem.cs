namespace Kentico.Xperience.Shopify.ShoppingCart
{
    /// <summary>
    /// Item in a shopping cart.
    /// </summary>
    public class ShoppingCartItem
    {
        /// <summary>
        /// ID of the shopping cart item.
        /// </summary>
        public required string ShopifyCartItemId { get; set; }


        /// <summary>
        /// Name of the shopping cart item.
        /// </summary>
        public required string Name { get; set; }


        /// <summary>
        /// Price of the shopping cart item.
        /// </summary>
        public required decimal Price { get; set; }


        /// <summary>
        /// GraphQL ID of the shopping cart item variant.
        /// </summary>
        public required string VariantGraphQLId { get; set; }


        /// <summary>
        /// Quantity of the shopping cart item.
        /// </summary>
        public required int Quantity { get; set; }


        /// <summary>
        /// List of coupon codes used for shopping cart item discounts.
        /// </summary>
        public IEnumerable<string>? DiscountCouponCodes { get; set; }


        /// <summary>
        /// Amount that was discounted from the cart item.
        /// </summary>
        public required decimal DiscountedAmount { get; set; }
    }
}
