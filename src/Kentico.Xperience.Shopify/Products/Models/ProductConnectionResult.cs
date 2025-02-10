using ShopifySharp.GraphQL;

namespace Kentico.Xperience.Shopify.Products.Models
{
    internal class ProductConnectionResult
    {
        public required ProductConnection Products { get; set; } = new();
    }
}
