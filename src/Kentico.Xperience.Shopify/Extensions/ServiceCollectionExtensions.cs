using Kentico.Xperience.Shopify.Config;
using Kentico.Xperience.Shopify.ProductSynchronization;
using Kentico.Xperience.Shopify.Services;
using Kentico.Xperience.Shopify.Services.InventoryService;
using Kentico.Xperience.Shopify.Services.ProductService;
using Kentico.Xperience.Shopify.ShoppingCart;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ShopifySharp.Extensions.DependencyInjection;

namespace Kentico.Xperience.Shopify;
public static class ServiceCollectionExtensions
{
    public static void RegisterShopifyServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        // Get Shopify config from appsettings.json
        services.Configure<ShopifyConfig>(configuration.GetSection(ShopifyConfig.SECTION_NAME));

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
        services.AddScoped<IShoppingService, ShoppingService>();

        // Add Storefront API HTTP client
        services.AddHttpClient(ShopifyConstants.STOREFRONT_API_CLIENT_NAME, (sp, httpClient) =>
        {
            var config = sp.GetRequiredService<IOptionsMonitor<ShopifyConfig>>();
            var uriBuilder = new UriBuilder(config.CurrentValue.ShopifyUrl)
            {
                Path = $"api/{config.CurrentValue.StorefrontApiVersion}/graphql.json"
            };
            httpClient.BaseAddress = uriBuilder.Uri;
            httpClient.DefaultRequestHeaders.Add(ShopifyConstants.STOREFRONT_API_HEADER_TOKEN_NAME, config.CurrentValue.StorefrontApiToken);
        });
    }
}
