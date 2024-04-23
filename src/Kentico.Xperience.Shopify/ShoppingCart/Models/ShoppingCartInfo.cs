using ShopifySharp.GraphQL;

namespace Kentico.Xperience.Shopify.ShoppingCart;

public class ShoppingCartInfo
{
    public string CartId { get; set; }
    public string CartUrl { get; set; }
    public CurrencyCode Currency { get; set; }
    public IEnumerable<ShoppingCartItem> Items { get; set; }
    public decimal GrandTotal { get; set; }

    internal ShoppingCartInfo(CartObjectModel shopifyCart)
    {
        var cartItems = shopifyCart.Lines?.Edges.Select(x => x.Node) ?? [];

        CartId = shopifyCart.Id;
        CartUrl = shopifyCart.CheckoutUrl;
        Currency = shopifyCart.Cost.TotalAmount.CurrencyCode;
        Items = cartItems.Select(CreateShoppingCartItem);
        GrandTotal = shopifyCart.Cost.TotalAmount.Amount;
    }

    private ShoppingCartItem CreateShoppingCartItem(CartLineNode node)
    {
        var merchandise = node.Merchandise;
        string name = merchandise.Product.Title;
        if (string.Compare(merchandise.Title, "default title", StringComparison.InvariantCultureIgnoreCase) != 0)
        {
            name += $" ({merchandise.Title})";
        }

        return new ShoppingCartItem()
        {
            ShopifyCartItemId = node.Id,
            Name = name,
            Price = node.Cost.TotalAmount.Amount,
            VariantGraphQLId = merchandise.Id,
            Quantity = node.Quantity
        };
    }
}

public class ShoppingCartItem
{
    public required string ShopifyCartItemId { get; set; }
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public required string VariantGraphQLId { get; set; }
    public required int Quantity { get; set; }
}
