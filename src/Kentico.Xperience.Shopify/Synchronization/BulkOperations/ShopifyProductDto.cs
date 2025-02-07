using System.Text.Json.Serialization;

namespace Kentico.Xperience.Shopify.Synchronization.BulkOperations
{
    public class ShopifyProductDto : SynchronizationDtoBase
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("descriptionHtml")]
        public string DescriptionHtml { get; set; } = string.Empty;

        public List<ShopifyProductVariantDto> Variants { get; set; } = [];

        public List<ShopifyMediaImageDto> Images { get; set; } = [];
    }
}
