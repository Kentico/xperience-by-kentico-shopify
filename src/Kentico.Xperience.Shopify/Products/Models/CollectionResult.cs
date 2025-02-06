using ShopifySharp.GraphQL;

namespace Kentico.Xperience.Shopify.Products.Models
{
    internal class CollectionResult
    {
        public required Collection Collection { get; set; } = new();
    }
}
