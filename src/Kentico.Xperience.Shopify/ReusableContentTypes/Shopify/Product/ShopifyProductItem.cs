using Kentico.Xperience.Ecommerce.Common.ContentItemSynchronization;
using Kentico.Xperience.Shopify.ReusableContentTypes;

namespace Shopify.ContentTypes;

/// <summary>
/// Class to use different class name instead of generated <see cref="Product" /> content item class. 
/// Class name <seealso cref="ShopifySharp.Product"/> is already used in ShopifySharp NuGet package.
/// </summary>
public class ShopifyProductItem : Product, IContentItemBase
{
    public string DisplayName => Title;

    public string ContentTypeName => CONTENT_TYPE_NAME;

    public new IEnumerable<ShopifyProductVariantItem> Variants { get; set; } = Enumerable.Empty<ShopifyProductVariantItem>();

    public new IEnumerable<ShopifyImageItem> Images { get; set; } = Enumerable.Empty<ShopifyImageItem>();

    public string ShopifyObjectID => ShopifyProductID;

    public int ContentItemIdentifier => SystemFields.ContentItemID;
}
