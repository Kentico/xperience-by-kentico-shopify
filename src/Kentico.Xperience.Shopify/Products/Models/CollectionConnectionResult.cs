using ShopifySharp.GraphQL;

namespace Kentico.Xperience.Shopify.Products.Models
{
    internal class CollectionConnectionResult
    {
        public required CollectionConnection Collections { get; set; } = new();
    }
}
