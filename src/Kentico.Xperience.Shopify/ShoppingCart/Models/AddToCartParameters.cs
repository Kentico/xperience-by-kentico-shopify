namespace Kentico.Xperience.Shopify.ShoppingCart;

internal class AddToCartParameters
{
    public required string CartId { get; set; }
    public required AddToCartLines Lines { get; set; }
}

internal class AddToCartLines
{
    public required string MerchandiseId { get; set; }
    public required int Quantity { get; set; }
}
