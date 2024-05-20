using Kentico.Xperience.Ecommerce.Common.ContentItemSynchronization;
using Kentico.Xperience.Shopify.ReusableContentTypes;

namespace Shopify.ContentTypes;
/// <summary>
/// Shopify product variant content item.
/// </summary>
public class ShopifyProductVariantItem : ProductVariant, IContentItemBase
{
    /// <summary>
    /// The display name of the product variant.
    /// </summary>
    public string DisplayName => Title;


    /// <summary>
    /// The content type name.
    /// </summary>
    public string ContentTypeName => CONTENT_TYPE_NAME;


    /// <summary>
    /// The image of the product variant.
    /// </summary>
    public new IEnumerable<ShopifyImageItem> Image { get; set; } = Enumerable.Empty<ShopifyImageItem>();


    /// <summary>
    /// The Shopify object ID of the product variant.
    /// </summary>
    public string ShopifyObjectID => ShopifyVariantID;


    /// <summary>
    /// The content item identifier.
    /// </summary>
    public int ContentItemIdentifier => SystemFields.ContentItemID;
}
