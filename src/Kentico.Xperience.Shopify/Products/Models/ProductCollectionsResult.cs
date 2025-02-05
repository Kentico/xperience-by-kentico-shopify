using ShopifySharp.GraphQL;

namespace Kentico.Xperience.Shopify.Products.Models
{
    internal class ProductCollectionsResult
    {
        public required CollectionConnection Collections { get; set; } = new();
    }
}
