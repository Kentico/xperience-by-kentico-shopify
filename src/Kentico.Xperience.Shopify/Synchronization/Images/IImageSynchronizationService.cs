using Kentico.Xperience.Shopify.ReusableContentTypes;

using ShopifySharp;

namespace Kentico.Xperience.Shopify.Synchronization.Images;
internal interface IImageSynchronizationService
{
    /// <summary>
    /// Store product images retrieved from Shopify to content hub as <see cref="ShopifyImageItem" />
    /// </summary>
    /// <param name="shopifyImages">List of retrieved images from Shopify.</param>
    /// <param name="imagesCI">Existing image content items.</param>
    /// <param name="languageName">Content items language.</param>
    /// <param name="userID">User ID used to add/modify/delete content items.</param>
    /// <returns>Images that should be assigned to particular variants and products.</returns>
    Task<ImageSynchronizationResult> ProcessImages(IEnumerable<ProductImage> shopifyImages, IEnumerable<ShopifyImageItem>? imagesCI, string languageName, int userID);
}
