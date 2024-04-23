using CMS.ContentEngine;
using Kentico.Xperience.Ecommerce.Common.ContentItemSynchronization;
using Kentico.Xperience.Shopify.ReusableContentTypes;
using Shopify;

namespace Kentico.Xperience.Shopify.ProductSynchronization;
public class ImageSynchronizationItem : ContentItemSynchronizationBase, ISynchronizationItem<ShopifyImageItem>
{
    /// <summary>
    /// Image name.
    /// </summary>
    public string ImageName { get; set; } = string.Empty;

    /// <summary>
    /// Image.
    /// </summary>
    public ContentItemAssetMetadataWithSource? ImageAsset { get; set; }


    /// <summary>
    /// ImageDescription.
    /// </summary>
    public string ImageDescription { get; set; } = string.Empty;


    /// <summary>
    /// ShopifyImageID.
    /// </summary>
    public string ShopifyImageID { get; set; } = string.Empty;


    public override string ContentTypeName => Image.CONTENT_TYPE_NAME;

    protected override string DisplayNameInternal => ImageName;

    public bool GetModifiedProperties(ShopifyImageItem contentItem, out Dictionary<string, object?> modifiedProps)
    {
        modifiedProps = [];
        if (contentItem == null)
        {
            return true;
        }

        if (ImageDescription != contentItem.ImageDescription)
        {
            modifiedProps.TryAdd(nameof(ShopifyImageItem.ImageDescription), ImageDescription);
        }
        if (ShopifyImageID != contentItem.ShopifyImageID)
        {
            modifiedProps.TryAdd(nameof(ShopifyImageItem.ShopifyImageID), ShopifyImageID);
        }

        return modifiedProps.Keys.Any();
    }
}
