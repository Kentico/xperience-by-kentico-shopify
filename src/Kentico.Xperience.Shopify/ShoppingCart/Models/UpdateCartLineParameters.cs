namespace Kentico.Xperience.Shopify.ShoppingCart;

internal class UpdateCartLineParameters
{
    public required string CartId { get; set; }
    public required UpdateCartLine Lines { get; set; }
}
