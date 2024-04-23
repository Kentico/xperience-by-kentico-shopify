using Kentico.Xperience.Ecommerce.Common.ContentItemSynchronization;

using Shopify;

namespace Kentico.Xperience.Shopify.ReusableContentTypes;
public class ShopifyImageItem : Image, IContentItemBase
{
    public string ContentTypeName => CONTENT_TYPE_NAME;

    public string DisplayName => ImageName;

    public string ImageName { get; set; } = string.Empty;

    public string ShopifyObjectID => ShopifyImageID;

    public int ContentItemIdentifier => SystemFields.ContentItemID;
}
