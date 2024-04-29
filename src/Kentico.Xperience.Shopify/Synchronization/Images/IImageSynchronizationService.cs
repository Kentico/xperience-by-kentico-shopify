using Kentico.Xperience.Shopify.ReusableContentTypes;

using ShopifySharp;

namespace Kentico.Xperience.Shopify.Synchronization.Images;
internal interface IImageSynchronizationService
{
    /// <summary>
    /// Store product images retrieved from Shopify to content hub as <see cref="ShopifyImageItem" />
    /// </summary>
    /// <param name="shopifyImages"></param>
    /// <param name="imagesCI"></param>
    /// <param name="languageName"></param>
    /// <param name="userID"></param>
    /// <returns></returns>
    Task<ImageSynchronizationResult> ProcessImages(IEnumerable<ProductImage> shopifyImages, IEnumerable<ShopifyImageItem>? imagesCI, string languageName, int userID);
}
