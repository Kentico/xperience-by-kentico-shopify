using CMS.Helpers;

using Kentico.Xperience.Shopify.Config;
using Kentico.Xperience.Shopify.Products.Models;

using ShopifySharp;
using ShopifySharp.Factories;
using ShopifySharp.GraphQL;

namespace Kentico.Xperience.Shopify.Products;

internal class ShopifyPriceService : ShopifyServiceBase, IShopifyPriceService
{
    private readonly IGraphService graphService;
    private readonly IShopifyIntegrationSettingsService settingsService;

    public ShopifyPriceService(
        IShopifyIntegrationSettingsService integrationSettingsService,
        IGraphServiceFactory graphServiceFactory,
        IShopifyIntegrationSettingsService settingsService)
        : base(integrationSettingsService)
    {
        graphService = graphServiceFactory.Create(shopifyCredentials);
        this.settingsService = settingsService;
    }

    public async Task<IDictionary<string, ProductPriceModel>> GetProductsPrice(IEnumerable<string> shopifyProductIds, CountryCode countryCode)
    {
        return await TryCatch(
            async () => await GetProductsPriceInternal(shopifyProductIds, countryCode),
            () => new Dictionary<string, ProductPriceModel>());
    }

    private async Task<IDictionary<string, ProductPriceModel>> GetProductsPriceInternal(IEnumerable<string> shopifyProductIds, CountryCode countryCode)
    {
        string currencyCode = settingsService.GetWebsiteChannelSettings()?.CurrencyCode ?? string.Empty;
        var productsQuery = shopifyProductIds.Select(x => $"id:{x}")
            .Join(" OR ");

        var request = new GraphRequest
        {
            Query = "query getProductPrices($query:String!,$country:CountryCode){products(first:250,query:$query){nodes{id hasOnlyDefaultVariant contextualPricing(context:{country:$country}){minVariantPricing{price{amount currencyCode}compareAtPrice{amount currencyCode}}}}}}",
            Variables = new Dictionary<string, object>
            {
                ["query"] = productsQuery,
                ["country"] = countryCode.ToStringRepresentation()
            }
        };

        var result = await graphService.PostAsync<ProductConnectionResult>(request);
        var products = result.Data.Products.nodes;

        var dict = new Dictionary<string, ProductPriceModel>();
        if (products == null)
        {
            return dict;
        }

        foreach (var product in products)
        {
            if (product == null || string.IsNullOrEmpty(product.id) || product.contextualPricing?.minVariantPricing == null)
            {
                continue;
            }

            dict.TryAdd(product.id.Split('/')[^1], new ProductPriceModel()
            {
                Price = product.contextualPricing.minVariantPricing.price?.amount ?? 0,
                ListPrice = product.contextualPricing.minVariantPricing.compareAtPrice?.amount,
                HasMultipleVariants = product.hasOnlyDefaultVariant ?? false
            });
        }

        return dict;
    }
}
