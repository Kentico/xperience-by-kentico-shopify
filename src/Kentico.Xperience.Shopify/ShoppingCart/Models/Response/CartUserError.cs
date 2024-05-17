namespace Kentico.Xperience.Shopify.ShoppingCart
{
    internal class CartUserError
    {
        public IEnumerable<string>? Field { get; set; }
        public string? Message { get; set; }
    }
}
