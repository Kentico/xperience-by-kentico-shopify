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
    /// The Shopify object ID associated with the image.
    /// </summary>
    public required string ParentID { get; set; }

    /// <summary>
    /// True if image belongs to specific product variant
    /// instead of product itself.
    /// </summary>
    public required bool IsVariantImage { get; set; }
}
