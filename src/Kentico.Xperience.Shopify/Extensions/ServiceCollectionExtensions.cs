using Kentico.Xperience.Shopify.Activities;
using Kentico.Xperience.Shopify.Config;
using Kentico.Xperience.Shopify.Orders;
using Kentico.Xperience.Shopify.Products;
using Kentico.Xperience.Shopify.ShoppingCart;
using Kentico.Xperience.Shopify.Synchronization;
using Kentico.Xperience.Shopify.Synchronization.Images;
using Kentico.Xperience.Shopify.Synchronization.Products;
using Kentico.Xperience.Shopify.Synchronization.Variants;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShopifySharp.Extensions.DependencyInjection;

namespace Kentico.Xperience.Shopify;
public static class ServiceCollectionExtensions
{
    public static void RegisterShopifyServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        // ShopifySharp dependency injection
        services.AddShopifySharpServiceFactories();

        services.AddScoped<IShopifyProductService, ShopifyProductService>();
        services.AddScoped<IShopifyCollectionService, ShopifyCollectionService>();
        services.AddScoped<IShopifyCurrencyService, ShopifyCurrencyService>();
        services.AddScoped<IShopifyInventoryService, ShopifyInventoryService>();
        services.AddScoped<IShopifyContentItemService, ShopifyContentItemService>();
        services.AddScoped<IShopifyPriceService, ShopifyPriceService>();
        services.AddScoped<IImageSynchronizationService, ImageSynchronizationService>();
        services.AddScoped<IProductSynchronizationService, ProductSynchronizationService>();
        services.AddScoped<IVariantSynchronizationService, VariantSynchronizationService>();
        services.AddScoped<IGraphQLHttpClientFactory, GraphQLHttpClientFactory>();
        services.AddScoped<IShoppingCartCacheService, ShoppingCartCacheService>();
        services.AddScoped<IShoppingService, ShoppingService>();
        services.AddScoped<IShopifyIntegrationSettingsService, ShopifyIntegrationSettingsService>();
        services.AddScoped<IEcommerceActivityLogger, EcommerceActivityLogger>();
        services.AddScoped<IShopifyOrderService, ShopifyOrderService>();

        services.AddSingleton<IShopifyCurrencyFormatService, ShopifyCurrencyFormatService>();

        // Add Storefront API HTTP client
        services.AddHttpClient(ShopifyConstants.STOREFRONT_API_CLIENT_NAME, SetupHttpClient);

        // Add options monitor
        services.Configure<ShopifyConfig>(builder.Configuration.GetSection(ShopifyConfig.SECTION_NAME));
    }

    private static void SetupHttpClient(IServiceProvider sp, HttpClient httpClient)
    {
        var scope = sp.CreateScope();
        var settings = scope.ServiceProvider.GetRequiredService<IShopifyIntegrationSettingsService>().GetSettings();

        if (settings == null || !Uri.TryCreate(settings.ShopifyUrl, UriKind.Absolute, out var uri))
        {
            return;
        }

        var uriBuilder = new UriBuilder(uri!)
        {
            Path = $"api/{settings.StorefrontApiVersion}/graphql.json"
        };
        httpClient.BaseAddress = uriBuilder.Uri;
        httpClient.DefaultRequestHeaders.Add(ShopifyConstants.STOREFRONT_API_HEADER_TOKEN_NAME, settings.StorefrontApiKey);
    }
}
