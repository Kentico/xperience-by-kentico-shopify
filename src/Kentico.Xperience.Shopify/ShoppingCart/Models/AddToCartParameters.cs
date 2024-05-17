namespace Kentico.Xperience.Shopify.ShoppingCart;

internal class AddToCartParameters
{
    public required string CartId { get; set; }
    public required AddToCartLines Lines { get; set; }
}
