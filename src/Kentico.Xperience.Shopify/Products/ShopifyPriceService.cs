using Kentico.Xperience.Shopify.Config;
using Kentico.Xperience.Shopify.Products.Models;

using ShopifySharp;
using ShopifySharp.Factories;
using ShopifySharp.Filters;
using ShopifySharp.GraphQL;

namespace Kentico.Xperience.Shopify.Products;

internal class ShopifyPriceService : ShopifyServiceBase, IShopifyPriceService
{
    private readonly IProductService productService;

    public ShopifyPriceService(IShopifyIntegrationSettingsService integrationSettingsService, IProductServiceFactory productServiceFactory)
        : base(integrationSettingsService)
    {
        productService = productServiceFactory.Create(shopifyCredentials);
    }

    public async Task<IDictionary<string, ProductPriceModel>> GetProductsPrice(IEnumerable<string> shopifyProductIds)
    {
        return await TryCatch(
            async () => await GetProductsPriceInternal(shopifyProductIds),
            () => new Dictionary<string, ProductPriceModel>());
    }

    private async Task<IDictionary<string, ProductPriceModel>> GetProductsPriceInternal(IEnumerable<string> shopifyProductIds)
    {
        string currency = CurrencyCode.CZK.ToString();
        var dict = new Dictionary<string, ProductPriceModel>();

        var filter = new ProductListFilter()
        {
            Ids = shopifyProductIds.Select(long.Parse),
            Fields = "Variants,Id",
            PresentmentCurrencies = [currency]
        };
        var result = await productService.ListAsync(filter, true);

        foreach (var product in result.Items)
        {
            if (product == null || !product.Id.HasValue || !product.Variants.Any())
            {
                continue;
            }

            var prices = product.Variants.Select(x => x.PresentmentPrices.FirstOrDefault(x => x.Price.CurrencyCode.Equals(currency, StringComparison.Ordinal)));

            dict.TryAdd(product.Id.Value.ToString(), new ProductPriceModel()
            {
                Price = prices.Min(x => x?.Price.Amount ?? decimal.MaxValue),
                ListPrice = prices.Min(x => x?.CompareAtPrice?.Amount),
                HasMultipleVariants = product.Variants.Count() > 1
            });
        }

        return dict;
    }
}
