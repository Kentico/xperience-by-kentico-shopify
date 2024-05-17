using Kentico.Xperience.Ecommerce.Common.ContentItemSynchronization;

using Shopify;

namespace Kentico.Xperience.Shopify.ReusableContentTypes;

/// <summary>
/// Shopify image content item.
/// </summary>
public class ShopifyImageItem : Image, IContentItemBase
{
    /// <summary>
    /// The name of the content type.
    /// </summary>
    public string ContentTypeName => CONTENT_TYPE_NAME;


    /// <summary>
    /// The display name of the image.
    /// </summary>
    public string DisplayName => ImageName;


    /// <summary>
    /// The name of the image.
    /// </summary>
    public string ImageName { get; set; } = string.Empty;


    /// <summary>
    /// The Shopify object ID.
    /// </summary>
    public string ShopifyObjectID => ShopifyImageID;


    /// <summary>
    /// The content item identifier.
    /// </summary>
    public int ContentItemIdentifier => SystemFields.ContentItemID;
}
