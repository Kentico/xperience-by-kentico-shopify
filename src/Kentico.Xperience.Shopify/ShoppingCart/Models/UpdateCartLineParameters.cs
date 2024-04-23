namespace Kentico.Xperience.Shopify.ShoppingCart;

internal class UpdateCartLineParameters
{
    public required string CartId { get; set; }
    public required UpdateCartLine Lines { get; set; }
}

internal class UpdateCartLine
{
    public required string Id { get; set; }
    public required int Quantity { get; set; }
}
