namespace Kentico.Xperience.Shopify.Synchronization.Products
{
    internal class ProductSynchronizationResult
    {
        public required bool NewProductCreated { get; set; }
        public required int ProductContentItemID { get; set; }
    }
}
