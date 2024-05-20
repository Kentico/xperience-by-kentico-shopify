using Kentico.Xperience.Ecommerce.Common.ContentItemSynchronization;
using Kentico.Xperience.Shopify.ReusableContentTypes;

namespace Shopify.ContentTypes;

/// <summary>
/// Class to use different class name instead of generated <see cref="Product" /> content item class. 
/// Class name <seealso cref="ShopifySharp.Product"/> is already used in ShopifySharp NuGet package.
/// </summary>
public class ShopifyProductItem : Product, IContentItemBase
{
    /// <summary>
    /// Display name of the product.
    /// </summary>
    public string DisplayName => Title;


    /// <summary>
    /// Content type name.
    /// </summary>
    public string ContentTypeName => CONTENT_TYPE_NAME;


    /// <summary>
    /// Gets or sets the variants of the product.
    /// </summary>
    public new IEnumerable<ShopifyProductVariantItem> Variants { get; set; } = Enumerable.Empty<ShopifyProductVariantItem>();


    /// <summary>
    /// Images of the product.
    /// </summary>
    public new IEnumerable<ShopifyImageItem> Images { get; set; } = Enumerable.Empty<ShopifyImageItem>();


    /// <summary>
    /// Shopify object ID of the product.
    /// </summary>
    public string ShopifyObjectID => ShopifyProductID;


    /// <summary>
    /// Content item identifier.
    /// </summary>
    public int ContentItemIdentifier => SystemFields.ContentItemID;
}
