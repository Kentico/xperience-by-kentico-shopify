namespace Kentico.Xperience.Shopify.ShoppingCart;

internal class CreateCartParameters
{
    public CreateCartBuyer? BuyerIdentity { get; set; }
    public IEnumerable<CartLine>? Lines { get; set; }
}

internal class CreateCartBuyer
{
    public string? CountryCode { get; set; }
}

internal class CartLine
{
    public int Quantity { get; set; }
    public string? MerchandiseId { get; set; }
}
