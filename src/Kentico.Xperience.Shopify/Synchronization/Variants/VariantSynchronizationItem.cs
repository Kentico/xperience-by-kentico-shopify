﻿using CMS.ContentEngine;

using Kentico.Xperience.Ecommerce.Common.ContentItemSynchronization;

using Shopify;
using Shopify.ContentTypes;

namespace Kentico.Xperience.Shopify.Synchronization.Variants;
internal class VariantSynchronizationItem : ContentItemSynchronizationBase, ISynchronizationItem<ShopifyProductVariantItem>
{
    /// <summary>
    /// Shopify VariantID.
    /// </summary>
    public required string ShopifyVariantID { get; set; }


    /// <summary>
    /// Title.
    /// </summary>
    public required string Title { get; set; }


    /// <summary>
    /// SKU.
    /// </summary>
    public required string SKU { get; set; }


    /// <summary>
    /// Weight.
    /// </summary>
    public decimal Weight { get; set; }


    /// <summary>
    /// ShopifyMerchandiseID.
    /// </summary>
    public required string ShopifyMerchandiseID { get; set; }


    /// <summary>
    /// Related product ID.
    /// </summary>
    public required string ShopifyProductID { get; set; }


    /// <summary>
    /// Image.
    /// </summary>
    public IEnumerable<ContentItemReference> Image { get; set; } = Enumerable.Empty<ContentItemReference>();

    public override string ContentTypeName => ProductVariant.CONTENT_TYPE_NAME;

    protected override string DisplayNameInternal => Title;

    public bool GetModifiedProperties(ShopifyProductVariantItem contentItem, out Dictionary<string, object?> modifiedProps)
    {
        modifiedProps = [];
        if (contentItem == null)
        {
            return true;
        }

        SetPropsIfDiff(contentItem.ShopifyVariantID, ShopifyVariantID, nameof(ShopifyProductVariantItem.ShopifyVariantID), modifiedProps);
        SetPropsIfDiff(contentItem.Title, Title, nameof(ShopifyProductVariantItem.Title), modifiedProps);
        SetPropsIfDiff(contentItem.SKU, SKU, nameof(ShopifyProductVariantItem.SKU), modifiedProps);
        SetPropsIfDiff(contentItem.Weight, Weight, nameof(ShopifyProductVariantItem.Weight), modifiedProps);
        SetPropsIfDiff(contentItem.ShopifyMerchandiseID, ShopifyMerchandiseID, nameof(ShopifyProductVariantItem.ShopifyMerchandiseID), modifiedProps);
        SetPropsIfDiff(contentItem.ShopifyProductID, ShopifyProductID, nameof(ShopifyProductVariantItem.ShopifyProductID), modifiedProps);

        if (ReferenceModified(contentItem.Image, Image))
        {
            modifiedProps.TryAdd(nameof(ShopifyProductVariantItem.Image), Image);
        }

        return modifiedProps.Count > 0;
    }
}
