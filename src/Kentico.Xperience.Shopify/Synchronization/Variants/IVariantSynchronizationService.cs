using Shopify.ContentTypes;

using ShopifySharp;

namespace Kentico.Xperience.Shopify.Synchronization.Variants;
internal interface IVariantSynchronizationService
{
    Task<IEnumerable<Guid>> ProcessVariants(
        IEnumerable<ProductVariant> variants,
        IEnumerable<ShopifyProductVariantItem>? existingVariants,
        Dictionary<string, Guid> variantImages,
        string languageName,
        string workspaceName,
        int userID);
}
