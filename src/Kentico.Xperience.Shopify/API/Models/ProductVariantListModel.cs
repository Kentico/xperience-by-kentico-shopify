namespace Kentico.Xperience.Shopify.API.Models;

public class ProductVariantListModel
{
    public required string PriceFormatted { get; set; }
    public string? ListPriceFormatted { get; set; }
    public required string StockCssClass { get; set; }
    public required string StockStatusText { get; set; }
    public required string MerchandiseID { get; set; }
    public int ItemsInCart { get; set; } = 0;
    public bool Available { get; set; }
}

