namespace Kentico.Xperience.Shopify.Products.Models;

/// <summary>
/// Model for product variant list items.
/// </summary>
public class ProductVariantListModel
{
    /// <summary>
    /// The formatted price of the variant.
    /// </summary>
    public required string PriceFormatted { get; set; }


    /// <summary>
    /// The formatted list price of the variant.
    /// </summary>
    public string? ListPriceFormatted { get; set; }


    /// <summary>
    /// The CSS class for the stock status.
    /// </summary>
    public required string StockCssClass { get; set; }


    /// <summary>
    /// The text indicating the stock status.
    /// </summary>
    public required string StockStatusText { get; set; }


    /// <summary>
    /// Product variant GraphQL ID.
    /// </summary>
    public required string MerchandiseID { get; set; }


    /// <summary>
    /// The number of items in the cart for this variant.
    /// </summary>
    public int ItemsInCart { get; set; } = 0;


    /// <summary>
    /// Indicates if the variant is available.
    /// </summary>
    public bool Available { get; set; }
}

