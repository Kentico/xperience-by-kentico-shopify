﻿using Shopify.ContentTypes;
using ShopifySharp;

namespace Kentico.Xperience.Shopify.ProductSynchronization;
public interface IProductSynchronizationService
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
    /// <returns></returns>
    Task ProcessProduct(Product product, IEnumerable<Guid> variants, IEnumerable<Guid> images, string languageName, int userID, ShopifyProductItem? existingProduct);

    /// <summary>
    /// Delete product content items from <paramref name="contentItemProducts"/> that are not in <paramref name="shopifyProducts"/>.
    /// Method also deletes all product variants and images(for both variants and product).
    /// </summary>
    /// <param name="contentItemProducts">Products retrieved from content hub</param>
    /// <param name="shopifyProducts">Products retrieved from Shopify</param>
    /// <param name="languageName"></param>
    /// <param name="userID"></param>
    /// <returns></returns>
    Task DeleteNonExistingProducts(IEnumerable<ShopifyProductItem> contentItemProducts, IEnumerable<Product> shopifyProducts, string languageName, int userID);
}
