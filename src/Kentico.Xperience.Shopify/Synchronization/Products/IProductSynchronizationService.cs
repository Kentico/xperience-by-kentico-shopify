using Kentico.Xperience.Shopify.Synchronization.BulkOperations;

using Shopify.ContentTypes;

namespace Kentico.Xperience.Shopify.Synchronization.Products;
internal interface IProductSynchronizationService
{
    /// <summary>
    /// Process retrieved product from Shopify and create/update products in kentico.
    /// </summary>
    /// <param name="product">Product retrieved from shopify</param>
    /// <param name="variants">Product variant content item GUIDs</param>
    /// <param name="images">Product images content item GUIDs</param>
    /// <param name="languageName">Language name</param>
    /// <param name="userID">User ID</param>
    /// <param name="existingProduct">Shopify product equivallent stored as content item</param>
    Task ProcessProduct(ShopifyProductDto product, IEnumerable<Guid> variants, IEnumerable<Guid> images, string languageName, int userID, ShopifyProductItem? existingProduct);

    /// <summary>
    /// Delete product content items from <paramref name="contentItemProducts"/> that are not in <paramref name="shopifyProducts"/>.
    /// Method also deletes all product variants and images(for both variants and product).
    /// </summary>
    /// <param name="contentItemProducts">Products retrieved from content hub</param>
    /// <param name="shopifyProducts">Products retrieved from Shopify</param>
    /// <param name="languageName">Content items language.</param>
    /// <param name="userID">User ID used to delete content items.</param>
    Task DeleteNonExistingProducts(IEnumerable<ShopifyProductItem> contentItemProducts, IEnumerable<ShopifyProductDto> shopifyProducts, string languageName, int userID);
}
