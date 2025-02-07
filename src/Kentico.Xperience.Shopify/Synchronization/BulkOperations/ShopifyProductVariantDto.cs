using System.Text.Json.Serialization;

using ShopifySharp.GraphQL;

namespace Kentico.Xperience.Shopify.Synchronization.BulkOperations
{
    public class ShopifyProductVariantDto : SynchronizationDtoBase
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("sku")]
        public string Sku { get; set; } = string.Empty;

        [JsonPropertyName("position")]
        public int Position { get; set; }

        [JsonPropertyName("inventoryItem")]
        public InventoryItem? InventoryItem { get; set; }

        public List<ShopifyMediaImageDto> Images { get; set; } = [];
    }
}
