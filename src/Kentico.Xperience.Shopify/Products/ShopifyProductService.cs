using System.Text;

using CMS.Helpers;

using Kentico.Xperience.Shopify.Config;
using Kentico.Xperience.Shopify.Products.Models;
using Kentico.Xperience.Shopify.ShoppingCart;

using ShopifySharp;
using ShopifySharp.Factories;
using ShopifySharp.GraphQL;

using ProductVariantInventoryPolicy = ShopifySharp.GraphQL.ProductVariantInventoryPolicy;
using ProductVariant = ShopifySharp.GraphQL.ProductVariant;


namespace Kentico.Xperience.Shopify.Products
{
    internal class ShopifyProductService : ShopifyServiceBase, IShopifyProductService
    {
        private readonly IShoppingService shoppingService;
        private readonly IGraphService graphService;

        private readonly IProductService productService;

        private const CurrencyCode defaultCurrency = CurrencyCode.USD;

        public ShopifyProductService(
            IShopifyIntegrationSettingsService integrationSettingsService,
            IGraphServiceFactory graphServiceFactory,
            IProductServiceFactory productServiceFactory,
            IShoppingService shoppingService) : base(integrationSettingsService)
        {
            this.shoppingService = shoppingService;

            graphService = graphServiceFactory.Create(shopifyCredentials);
            productService = productServiceFactory.Create(shopifyCredentials);
        }

        public async Task<ListResultWrapper<ShopifyProductListModel>> GetProductsAsync(ProductFilter? initialFilter = null)
        {
            return await TryCatch(
                async () => CreateResultModel(await GetProductsAsyncInternal(initialFilter ?? new())),
                GenerateEmptyResult);
        }

        public async Task<Dictionary<string, ProductVariantListModel>> GetProductVariants(string shopifyProductID, CountryCode countryCode)
        {
            return await TryCatch(
                async () => await GetProductVariantsInternal(shopifyProductID, countryCode),
                () => []);
        }

        //public Task<ListResultWrapper<ShopifySharp.Product>> GetAllProductsRaw(ProductFilter? initialFilter = null)
        //{
        //    // TODO Get products raw
        //    return Task.FromResult(new ListResultWrapper<ShopifySharp.Product>());
        //}

        public async Task<ShopifySharp.Lists.ListResult<ShopifySharp.Product>> GetAllProductsRaw()
        {
            return await TryCatch(
                async () => await productService.ListAsync(),
                () => new ShopifySharp.Lists.ListResult<ShopifySharp.Product>([], null));
        }

        private async Task<Dictionary<string, ProductVariantListModel>> GetProductVariantsInternal(string shopifyProductID, CountryCode countryCode)
        {
            var cart = await shoppingService.GetCurrentShoppingCart();
            var variants = await GetVariantsFromApi(shopifyProductID, countryCode);
            if (cart != null)
            {
                foreach (var variant in variants.Values)
                {
                    var cartItem = cart.Items.FirstOrDefault(x => x.VariantGraphQLId.Equals(variant.MerchandiseID, StringComparison.Ordinal));
                    variant.ItemsInCart = cartItem?.Quantity ?? 0;
                }
            }

            return variants;
        }

        private async Task<ProductConnection> GetProductsAsyncInternal(ProductFilter initialFilter)
        {
            var queryFilter = new StringBuilder();
            if (initialFilter.CollectionID != null)
            {
                queryFilter.Append($"collection_id:{initialFilter.CollectionID} ");
            }
            if (initialFilter.Ids != null && initialFilter.Ids.Any())
            {
                var ids = initialFilter.Ids.Select(x => $"id:{x}")
                    .Join(" OR ");

                queryFilter.Append("AND (", ids, ")");
            }

            var request = new GraphRequest
            {
                Query = "query getProductsByCollection($query:String!,$topN:Int,$country:CountryCode,$startCursor:String,$endCursor:String){products(first:$topN,query:$query,after:$endCursor,before:$startCursor){nodes{id contextualPricing(context:{country:$country}){minVariantPricing{price{amount currencyCode}compareAtPrice{amount currencyCode}}}featuredMedia{...on MediaImage{__typename image{altText url}}}title description onlineStorePreviewUrl}pageInfo{endCursor startCursor}}}",
                Variables = new Dictionary<string, object>
                {
                    ["topN"] = initialFilter.Limit ?? 250,
                    ["country"] = initialFilter.Country,
                    ["query"] = queryFilter.ToString(),
                }
            };

            if (!string.IsNullOrEmpty(initialFilter.StartCursor))
            {
                request.Variables.Add("startCursor", initialFilter.StartCursor);
            }
            if (!string.IsNullOrEmpty(initialFilter.EndCursor))
            {
                request.Variables.Add("endCursor", initialFilter.EndCursor);
            }

            var result = await graphService.PostAsync<ProductConnectionResult>(request);

            return result.Data.Products ?? new();
        }

        private ListResultWrapper<ShopifyProductListModel> CreateResultModel(ProductConnection products)
        {
            var items = new List<ShopifyProductListModel>();
            foreach (var product in products.nodes ?? [])
            {
                var firstImage = product.featuredMedia?.AsMediaImage()?.image;
                var pricing = product.contextualPricing?.minVariantPricing;
                var currencyCode = pricing?.price?.currencyCode ?? defaultCurrency;
                var price = pricing?.price?.amount;
                var listPrice = pricing?.compareAtPrice?.amount;

                items.Add(new ShopifyProductListModel
                {
                    Image = firstImage?.url,
                    ImageAlt = firstImage?.altText,
                    Name = product.title,
                    Description = product.description,
                    ShopifyUrl = product.onlineStorePreviewUrl,
                    Price = price,
                    ListPrice = listPrice,
                    PriceFormatted = price.FormatPrice(currencyCode),
                    ListPriceFormatted = listPrice.FormatPrice(currencyCode),
                    HasMoreVariants = product.hasOnlyDefaultVariant ?? false
                });
            }

            return new ListResultWrapper<ShopifyProductListModel>()
            {
                Items = items,
                StartCursor = products.pageInfo?.startCursor,
                EndCursor = products.pageInfo?.endCursor
            };
        }

        private ListResultWrapper<ShopifyProductListModel> GenerateEmptyResult()
        {
            return new ListResultWrapper<ShopifyProductListModel>()
            {
                Items = Enumerable.Empty<ShopifyProductListModel>()
            };
        }

        private async Task<Dictionary<string, ProductVariantListModel>> GetVariantsFromApi(string shopifyProductID, CountryCode countryCode)
        {
            var request = new GraphRequest
            {
                Query = "query productVariants($query:String,$country:CountryCode){productVariants(first:250,query:$query){nodes{id inventoryPolicy inventoryQuantity inventoryItem{tracked} contextualPricing(context:{country:$country}){price{amount currencyCode}compareAtPrice{amount currencyCode}}}}}",
                Variables = new Dictionary<string, object>()
                {
                    ["query"] = $"product_id:{shopifyProductID}",
                    ["country"] = countryCode.ToStringRepresentation()
                }
            };

            var result = await graphService.PostAsync<ProductVariantConnectionResult>(request);

            var variants = result.Data.ProductVariants.nodes ?? [];

            return variants.ToDictionary(x => x.id?.Split('/')[^1] ?? string.Empty, x => GetVariantListModel(x));
        }

        private ProductVariantListModel GetVariantListModel(ProductVariant variant)
        {
            bool sellIfNotInStock = ProductVariantInventoryPolicy.CONTINUE == variant.inventoryPolicy;
            var price = variant.contextualPricing?.price;
            var listPrice = variant.contextualPricing?.compareAtPrice;
            var inventoryItem = variant.inventoryItem;

            bool inStock = sellIfNotInStock || !(inventoryItem?.tracked ?? false) || variant.inventoryQuantity > 0;

            return new ProductVariantListModel()
            {
                PriceFormatted = price?.amount.FormatPrice(price.currencyCode ?? defaultCurrency) ?? string.Empty,
                ListPriceFormatted = listPrice?.amount.FormatPrice(listPrice.currencyCode ?? defaultCurrency) ?? string.Empty,
                StockCssClass = inStock ? "available" : "unavailable",
                StockStatusText = inStock ? "In stock" : "Out of stock",
                Available = inStock,
                MerchandiseID = variant.id!,
            };
        }
    }
}
