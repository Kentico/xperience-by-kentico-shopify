using CMS;
using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using Kentico.Xperience.Shopify.ProductSynchronization;

[assembly: RegisterModule(typeof(ShopifyProductSynchronizationModule))]

namespace Kentico.Xperience.Shopify.ProductSynchronization;
internal class ShopifyProductSynchronizationModule : Module
{
    public ShopifyProductSynchronizationModule() : base(nameof(ShopifyProductSynchronizationModule))
    {
    }

    protected override void OnInit(ModuleInitParameters parameters)
    {
        base.OnInit(parameters);

        RequestEvents.RunEndRequestTasks.Execute += (_, _) => ShopifyProductSynchronizationWorker.Current.EnsureRunningThread();
    }
}
