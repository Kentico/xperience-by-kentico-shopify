using Kentico.Xperience.Shopify.ReusableContentTypes;
using Kentico.Xperience.Shopify.Synchronization.BulkOperations;

namespace Kentico.Xperience.Shopify.Synchronization.Images;
internal interface IImageSynchronizationService
{
    /// <summary>
    /// Store product images retrieved from Shopify to content hub as <see cref="ShopifyImageItem" />
    /// </summary>
    /// <param name="shopifyProduct">Retrieved product from Shopify.</param>
    /// <param name="imagesCI">Existing image content items.</param>
    /// <param name="languageName">Content items language.</param>
    /// <param name="userID">User ID used to add/modify/delete content items.</param>
    /// <returns>Images that should be assigned to particular variants and products.</returns>
    Task<ImageSynchronizationResult> ProcessImages(ShopifyProductDto shopifyProduct, IEnumerable<ShopifyImageItem>? imagesCI, string languageName, int userID);
}
