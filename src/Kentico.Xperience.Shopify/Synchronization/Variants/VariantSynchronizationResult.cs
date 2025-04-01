namespace Kentico.Xperience.Shopify.Synchronization.Variants
{
    internal class VariantSynchronizationResult
    {
        public required IEnumerable<Guid> ProductVariantGuids { get; set; }
        public required IEnumerable<int> CreatedVariantContentItemIDs { get; set; }
    }
}
