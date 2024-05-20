using ShopifySharp.GraphQL;

namespace Kentico.Xperience.Shopify.ShoppingCart;

/// <summary>
/// Information about a shopping cart.
/// </summary>
public class ShoppingCartInfo
{
    /// <summary>
    /// The ID of the shopping cart.
    /// </summary>
    public string CartId { get; set; }

    /// <summary>
    /// The URL of the shopping cart.
    /// </summary>
    public string CartUrl { get; set; }

    /// <summary>
    /// The currency of the shopping cart.
    /// </summary>
    public CurrencyCode Currency { get; set; }

    /// <summary>
    /// The items in the shopping cart.
    /// </summary>
    public IEnumerable<ShoppingCartItem> Items { get; set; }

    /// <summary>
    /// The grand total of the shopping cart.
    /// </summary>
    public decimal GrandTotal { get; set; }

    /// <summary>
    /// The discount codes applied to the shopping cart.
    /// </summary>
    public IEnumerable<string> DiscountCodes { get; set; }


    internal ShoppingCartInfo(CartObjectModel shopifyCart)
    {
        var cartItems = shopifyCart.Lines?.Edges.Select(x => x.Node) ?? [];

        CartId = shopifyCart.Id;
        CartUrl = shopifyCart.CheckoutUrl;
        Currency = shopifyCart.Cost.TotalAmount.CurrencyCode;
        Items = cartItems.Select(CreateShoppingCartItem);
        GrandTotal = shopifyCart.Cost.TotalAmount.Amount;
        DiscountCodes = shopifyCart.DiscountCodes?.Where(x => x.Applicable).Select(x => x.Code) ?? [];
    }

    private ShoppingCartItem CreateShoppingCartItem(CartLineNode node)
    {
        var merchandise = node.Merchandise;
        string name = merchandise.Product.Title;
        if (string.Compare(merchandise.Title, ShopifyConstants.DEFAULT_VARIANT_NAME, StringComparison.InvariantCultureIgnoreCase) != 0)
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
