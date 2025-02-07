using System.Text.Json.Serialization;

using ShopifySharp.GraphQL;

namespace Kentico.Xperience.Shopify.Synchronization.BulkOperations
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "__typename")]
    [JsonDerivedType(typeof(ShopifyProductDto), typeDiscriminator: nameof(Product))]
    [JsonDerivedType(typeof(ShopifyProductVariantDto), typeDiscriminator: nameof(ProductVariant))]
    [JsonDerivedType(typeof(ShopifyMediaImageDto), typeDiscriminator: nameof(MediaImage))]
    public abstract class SynchronizationDtoBase
    {
        [JsonPropertyName("__parentId")]
        public string? ParentId { get; set; }

        [JsonPropertyName("__typename")]
        public string TypeName { get; set; } = string.Empty;

        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        public SynchronizationDtoBase? Parent { get; set; }
    }
}
