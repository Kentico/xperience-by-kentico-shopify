using ShopifySharp.GraphQL;

namespace Kentico.Xperience.Shopify.API.Models;

public class UpdateCartModel
{
    public CountryCode CountryCode { get; init; }
    public CartOperation CartOperation { get; set; }
    public long SelectedVariant { get; init; }
    public int VariantQuantity { get; init; }
    public string? SelectedVariantMerchandiseID { get; init; }
}

public enum CartOperation
{
    Add,
    Remove
}

