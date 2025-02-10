using System.Text.Json.Serialization;

using ShopifySharp.GraphQL;

namespace Kentico.Xperience.Shopify.Synchronization.BulkOperations
{
    /// <summary>
    /// Shopify Media Image class.
    /// </summary>
    public class ShopifyMediaImageDto : SynchronizationDtoBase
    {
        /// <summary>
        /// MediaImage retrieved from <c>media</c> field of the Shopify variant or product.
        /// </summary>
        [JsonPropertyName("image")]
        public required Image Image { get; set; }
    }
}
