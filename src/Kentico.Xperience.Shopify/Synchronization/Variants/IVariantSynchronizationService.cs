using Kentico.Xperience.Shopify.Synchronization.BulkOperations;

using Shopify.ContentTypes;

namespace Kentico.Xperience.Shopify.Synchronization.Variants;
internal interface IVariantSynchronizationService
{
    Task<IEnumerable<Guid>> ProcessVariants(
        IEnumerable<ShopifyProductVariantDto> variants,
        IEnumerable<ShopifyProductVariantItem>? existingVariants,
        Dictionary<string, Guid> variantImages,
        string languageName,
        string workspaceName,
        int userID);
}
