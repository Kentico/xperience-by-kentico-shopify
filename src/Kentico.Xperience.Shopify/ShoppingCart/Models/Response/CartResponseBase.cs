namespace Kentico.Xperience.Shopify.ShoppingCart;
internal class CartResponseBase
{
    public CartObjectModel? Cart { get; set; }
    public IEnumerable<CartUserError>? UserErrors { get; set; }
}

internal class CartUserError
{
    public IEnumerable<string>? Field { get; set; }
    public string? Message { get; set; }
}

