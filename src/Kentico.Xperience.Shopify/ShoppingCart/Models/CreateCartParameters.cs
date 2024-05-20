namespace Kentico.Xperience.Shopify.ShoppingCart;

internal class CreateCartParameters
{
    public CreateCartBuyer? BuyerIdentity { get; set; }
    public IEnumerable<CartLine>? Lines { get; set; }
}
