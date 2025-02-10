using System.Text.Json.Serialization;

namespace Kentico.Xperience.Shopify.Synchronization.BulkOperations
{
    /// <summary>
    /// Shopify product class.
    /// </summary>
    public class ShopifyProductDto : SynchronizationDtoBase
    {
        /// <summary>
        /// Product title.
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Product description.
        /// </summary>
        [JsonPropertyName("descriptionHtml")]
        public string DescriptionHtml { get; set; } = string.Empty;

        /// <summary>
        /// Product variants.
        /// </summary>
        public List<ShopifyProductVariantDto> Variants { get; set; } = [];

        /// <summary>
        /// Product images.
        /// </summary>
        public List<ShopifyMediaImageDto> Images { get; set; } = [];
    }
}
