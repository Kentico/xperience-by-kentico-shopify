using System.Text.Json.Serialization;

using ShopifySharp.GraphQL;

namespace Kentico.Xperience.Shopify.Synchronization.BulkOperations
{
    /// <summary>
    /// Shopify object base class. Used for deserialization from bulk requests.
    /// </summary>
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "__typename")]
    [JsonDerivedType(typeof(ShopifyProductDto), typeDiscriminator: nameof(Product))]
    [JsonDerivedType(typeof(ShopifyProductVariantDto), typeDiscriminator: nameof(ProductVariant))]
    [JsonDerivedType(typeof(ShopifyMediaImageDto), typeDiscriminator: nameof(MediaImage))]
    public abstract class SynchronizationDtoBase
    {
        /// <summary>
        /// Parent ID.
        /// </summary>
        [JsonPropertyName("__parentId")]
        public string? ParentId { get; set; }

        /// <summary>
        /// ID.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Parent object.
        /// </summary>
        public SynchronizationDtoBase? Parent { get; set; }
    }
}
