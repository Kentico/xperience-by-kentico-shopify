using System.Diagnostics;

using CMS.Base;
using CMS.Core;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kentico.Xperience.Shopify.Synchronization;
internal class ShopifySynchronizationWorker : ThreadWorker<ShopifySynchronizationWorker>
{
    protected override int DefaultInterval => _defaultInterval;

    private ILogger<ShopifySynchronizationWorker> logger = null!;
    private readonly int _defaultInterval = Convert.ToInt32(TimeSpan.FromMinutes(15).TotalMilliseconds);

    protected override void Initialize()
    {
        base.Initialize();
        logger = Service.Resolve<ILogger<ShopifySynchronizationWorker>>();
    }

    /// <summary>Method processing actions.</summary>
    protected override void Process()
    {
        Debug.WriteLine($"Worker {GetType().FullName} running");

        try
        {
            SynchronizeProducts();
            logger.LogInformation("Shopify product synchronization done.");
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Error occured during running '{GetType().Name}'");
        }
    }

    private static void SynchronizeProducts()
    {
        using var serviceScope = Service.Resolve<IServiceProvider>().CreateScope();
        var provider = serviceScope.ServiceProvider;
        var shopifySynchronizationWorkerService = provider.GetRequiredService<IShopifySynchronizationWorkerService>();
        shopifySynchronizationWorkerService.SynchronizeProducts().GetAwaiter().GetResult();
    }

    protected override void Finish()
    {
    }
}
