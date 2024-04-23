using CMS.ContentEngine;
using Kentico.Xperience.Ecommerce.Common.ContentItemSynchronization;
using Shopify;
using Shopify.ContentTypes;

namespace Kentico.Xperience.Shopify.ProductSynchronization;
public class ProductVariantSynchronizationItem : ContentItemSynchronizationBase, ISynchronizationItem<ShopifyProductVariantItem>
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

        if (ShopifyVariantID != contentItem.ShopifyVariantID)
        {
            modifiedProps.TryAdd(nameof(ShopifyProductVariantItem.ShopifyVariantID), ShopifyVariantID);
        }
        if (Title != contentItem.Title)
        {
            modifiedProps.TryAdd(nameof(ShopifyProductVariantItem.Title), Title);
        }
        if (SKU != contentItem.SKU)
        {
            modifiedProps.TryAdd(nameof(ShopifyProductVariantItem.SKU), SKU);
        }
        if (Weight != contentItem.Weight)
        {
            modifiedProps.TryAdd(nameof(ShopifyProductVariantItem.Weight), Weight);
        }
        if (ShopifyMerchandiseID != contentItem.ShopifyMerchandiseID)
        {
            modifiedProps.TryAdd(nameof(ShopifyProductVariantItem.ShopifyMerchandiseID), ShopifyMerchandiseID);
        }
        if (ReferenceModified(contentItem.Image, Image))
        {
            modifiedProps.TryAdd(nameof(ShopifyProductVariantItem.Image), Image);
        }

        return modifiedProps.Count > 0;
    }
}
