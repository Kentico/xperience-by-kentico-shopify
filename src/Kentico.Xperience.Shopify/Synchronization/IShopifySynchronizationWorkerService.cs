namespace Kentico.Xperience.Shopify.Synchronization;

/// <summary>
/// Product synchronization service for <see cref="ShopifySynchronizationWorker"/>
/// </summary>
public interface IShopifySynchronizationWorkerService
{
    /// <summary>
    /// Synchronize products with all dependencies.
    /// </summary>
    Task SynchronizeProducts();
}
