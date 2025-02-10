using CMS;
using CMS.Base;
using CMS.Core;
using CMS.DataEngine;

using Kentico.Xperience.Shopify.Admin;

using Microsoft.Extensions.DependencyInjection;

[assembly: RegisterModule(type: typeof(ShopifyModule))]

namespace Kentico.Xperience.Shopify.Admin;

internal class ShopifyModule : Module
{
    private IShopifyIntegrationSettingsModuleInstaller? installer;

    public ShopifyModule() : base(nameof(ShopifyModule))
    {
    }

    protected override void OnInit(ModuleInitParameters parameters)
    {
        base.OnInit(parameters);

        var services = parameters.Services;
        installer = services.GetRequiredService<IShopifyIntegrationSettingsModuleInstaller>();
        ApplicationEvents.Initialized.Execute += InitializedModule;
    }

    private void InitializedModule(object? sender, EventArgs e) => installer?.Install();
}
