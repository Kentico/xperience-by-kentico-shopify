using System.Text.Json.Serialization;

using ShopifySharp.GraphQL;

namespace Kentico.Xperience.Shopify.Synchronization.BulkOperations
{
    /// <summary>
    /// Shopify porduct variant class.
    /// </summary>
    public class ShopifyProductVariantDto : SynchronizationDtoBase
    {
        /// <summary>
        /// Title.
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// SKU.
        /// </summary>
        [JsonPropertyName("sku")]
        public string Sku { get; set; } = string.Empty;

        /// <summary>
        /// Position used to order the variants.
        /// </summary>
        [JsonPropertyName("position")]
        public int Position { get; set; }

        /// <summary>
        /// Inventory item.
        /// </summary>
        [JsonPropertyName("inventoryItem")]
        public InventoryItem? InventoryItem { get; set; }

        /// <summary>
        /// Images.
        /// </summary>
        public List<ShopifyMediaImageDto> Images { get; set; } = [];
    }
}
