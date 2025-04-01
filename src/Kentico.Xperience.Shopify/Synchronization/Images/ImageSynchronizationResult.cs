namespace Kentico.Xperience.Shopify.Synchronization.Images;
internal class ImageSynchronizationResult
{
    /// <summary>
    /// Dictionary where key is shopify variant ID and value is GUID of image content item.
    /// </summary>
    public Dictionary<string, Guid> VariantImages { get; set; } = [];

    /// <summary>
    /// Product images.
    /// </summary>
    public List<Guid> ProductImages { get; set; } = [];

    /// <summary>
    /// Content item IDs of created images.
    /// </summary>
    public IEnumerable<int> CreatedImages { get; set; } = [];
}
