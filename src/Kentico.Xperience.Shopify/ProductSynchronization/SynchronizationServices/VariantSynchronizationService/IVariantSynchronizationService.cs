using Shopify.ContentTypes;
using ShopifySharp;

namespace Kentico.Xperience.Shopify.ProductSynchronization;
public interface IVariantSynchronizationService
{
    Task<IEnumerable<Guid>> ProcessVariants(IEnumerable<ProductVariant> variants, IEnumerable<ShopifyProductVariantItem>? existingVariants, Dictionary<string, Guid> variantImages, string languageName, int userID);
}
