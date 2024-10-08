﻿using CMS.ContentEngine;

using Kentico.Xperience.Ecommerce.Common.ContentItemSynchronization;
using Kentico.Xperience.Shopify.ReusableContentTypes;

using Shopify;

namespace Kentico.Xperience.Shopify.Synchronization.Images;


/// <summary>
/// Image synchronization item.
/// </summary>
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


    /// <inheritdoc/>
    public override string ContentTypeName => Image.CONTENT_TYPE_NAME;


    /// <inheritdoc/>
    protected override string DisplayNameInternal => ImageName;


    /// <inheritdoc/>
    public bool GetModifiedProperties(ShopifyImageItem contentItem, out Dictionary<string, object?> modifiedProps)
    {
        modifiedProps = [];
        if (contentItem == null)
        {
            return true;
        }

        SetPropsIfDiff(contentItem.ImageDescription, ImageDescription, nameof(ShopifyImageItem.ImageDescription), modifiedProps);
        SetPropsIfDiff(contentItem.ShopifyImageID, ShopifyImageID, nameof(ShopifyImageItem.ShopifyImageID), modifiedProps);

        return modifiedProps.Keys.Any();
    }
}
