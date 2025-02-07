using System.Text.Json.Serialization;

using ShopifySharp.GraphQL;

namespace Kentico.Xperience.Shopify.Synchronization.BulkOperations
{
    public class ShopifyMediaImageDto : SynchronizationDtoBase
    {
        [JsonPropertyName("image")]
        public required Image Image { get; set; }
    }
}
