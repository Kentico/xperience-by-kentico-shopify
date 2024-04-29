using CMS.ContentEngine;

using Kentico.Xperience.Ecommerce.Common.ContentItemSynchronization;
using Kentico.Xperience.Ecommerce.Common.ContentItemSynchronization.Interfaces;

using Shopify;
using Shopify.ContentTypes;

namespace Kentico.Xperience.Shopify.Synchronization.Products;
public class ProductSynchronizationItem : ContentItemSynchronizationBase, ISynchronizationItem<ShopifyProductItem>
{
    public override string ContentTypeName => Product.CONTENT_TYPE_NAME;
    protected override string DisplayNameInternal => Title;

    /// <summary>
    /// Title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Shopify ProductID.
    /// </summary>
    public string ShopifyProductID { get; set; } = string.Empty;

    /// <summary>
    /// Images.
    /// </summary>
    public IEnumerable<ContentItemReference> Images { get; set; } = Enumerable.Empty<ContentItemReference>();

    /// <summary>
    /// Variants.
    /// </summary>
    public IEnumerable<ContentItemReference> Variants { get; set; } = Enumerable.Empty<ContentItemReference>();

    public bool GetModifiedProperties(ShopifyProductItem contentItem, out Dictionary<string, object?> modifiedProps)
    {
        modifiedProps = [];
        if (contentItem == null)
        {
            return true;
        }

        SetPropsIfDiff(contentItem.Title, Title, nameof(ShopifyProductItem.Title), modifiedProps);
        SetPropsIfDiff(contentItem.Description, Description, nameof(ShopifyProductItem.Description), modifiedProps);
        SetPropsIfDiff(contentItem.ShopifyProductID, ShopifyProductID, nameof(ShopifyProductItem.ShopifyProductID), modifiedProps);

        if (ReferenceModified(contentItem.Images, Images))
        {
            modifiedProps.TryAdd(nameof(ShopifyProductItem.Images), Images);
        }
        if (ReferenceModified(contentItem.Variants, Variants))
        {
            modifiedProps.TryAdd(nameof(ShopifyProductItem.Variants), Variants);
        }

        return modifiedProps.Count > 0;
    }
}
