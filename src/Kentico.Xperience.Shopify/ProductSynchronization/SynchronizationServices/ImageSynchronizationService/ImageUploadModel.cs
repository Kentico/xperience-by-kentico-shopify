namespace Kentico.Xperience.Shopify.ProductSynchronization;
public class ImageUploadModel
{
    public required string ImageUrl { get; set; }
    public required string ImageName { get; set; }
    public string Description { get; set; } = string.Empty;
    public required string ShopifyImageID { get; set; }
    public IEnumerable<string>? VariantIDs { get; set; }
}
