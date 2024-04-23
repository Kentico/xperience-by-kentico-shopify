using Kentico.Xperience.Ecommerce.Common.ContentItemSynchronization;
using Kentico.Xperience.Shopify.ReusableContentTypes;

namespace Shopify.ContentTypes;
public class ShopifyProductVariantItem : ProductVariant, IContentItemBase
{
    public string DisplayName => Title;
    public string ContentTypeName => CONTENT_TYPE_NAME;

    public new IEnumerable<ShopifyImageItem> Image { get; set; } = Enumerable.Empty<ShopifyImageItem>();

    public string ShopifyObjectID => ShopifyVariantID;

    public int ContentItemIdentifier => SystemFields.ContentItemID;
}
