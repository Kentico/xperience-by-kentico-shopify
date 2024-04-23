using CMS;
using CMS.Base;
using CMS.Core;
using CMS.DataEngine;

using Kentico.Xperience.Shopify.Synchronization;

[assembly: RegisterModule(typeof(ShopifySynchronizationModule))]

namespace Kentico.Xperience.Shopify.Synchronization;
internal class ShopifySynchronizationModule : Module
{
    public ShopifySynchronizationModule() : base(nameof(ShopifySynchronizationModule))
    {
    }

    protected override void OnInit(ModuleInitParameters parameters)
    {
        base.OnInit(parameters);

        RequestEvents.RunEndRequestTasks.Execute += (_, _) => ShopifySynchronizationWorker.Current.EnsureRunningThread();
    }
}
