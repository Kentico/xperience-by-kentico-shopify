namespace Kentico.Xperience.Shopify.Synchronization.Images;

/// <summary>
/// Model for uploading product/variant image.
/// </summary>
public class ImageUploadModel
{
    /// <summary>
    /// The URL of the image.
    /// </summary>
    public required string ImageUrl { get; set; }


    /// <summary>
    /// The name of the image.
    /// </summary>
    public required string ImageName { get; set; }


    /// <summary>
    /// The description of the image.
    /// </summary>
    public string Description { get; set; } = string.Empty;


    /// <summary>
    /// The ID of the image in Shopify.
    /// </summary>
    public required string ShopifyImageID { get; set; }


    /// <summary>
    /// Variant IDs that use the image. If image is assigned
    /// directly to product, array is empty.
    /// </summary>
    public string[] VariantIds { get; set; } = [];
}
