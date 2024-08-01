using Kentico.Xperience.Shopify.Config;
using Kentico.Xperience.Shopify.Products.Models;
using Kentico.Xperience.Shopify.ShoppingCart;

using ShopifySharp;
using ShopifySharp.Factories;
using ShopifySharp.Filters;
using ShopifySharp.Lists;


namespace Kentico.Xperience.Shopify.Products
{
    internal class ShopifyProductService : ShopifyServiceBase, IShopifyProductService
    {
        private readonly IProductService productService;
        private readonly IShopifyInventoryService inventoryService;
        private readonly IShoppingService shoppingService;
        private readonly IShopifyIntegrationSettingsService settingsService;

        private readonly Uri shopifyProductUrlBase;
        private readonly string[] _shopifyFields = ["title", "body_html", "handle", "images", "variants"];
        private const string defaultCurrency = "USD";

        private string ShopifyFields => string.Join(",", _shopifyFields);

        public ShopifyProductService(
            IShopifyIntegrationSettingsService integrationSettingsService,
            IProductServiceFactory productServiceFactory,
            IShopifyInventoryService inventoryService,
            IShoppingService shoppingService,
            IShopifyIntegrationSettingsService settingsService) : base(integrationSettingsService)
        {
            this.inventoryService = inventoryService;
            this.shoppingService = shoppingService;
            this.settingsService = settingsService;

            productService = productServiceFactory.Create(shopifyCredentials);

            var uriBuilder = new UriBuilder(shopifyCredentials.ShopDomain)
            {
                Path = "products"
            };
            shopifyProductUrlBase = uriBuilder.Uri;
        }

        public async Task<ListResultWrapper<ShopifyProductListModel>> GetProductsAsync(ProductFilter initialFilter)
        {
            return await TryCatch(
                async () => await GetProductsAsyncInternal(initialFilter),
                GenerateEmptyResult);
        }

        public async Task<ListResultWrapper<ShopifyProductListModel>> GetProductsAsync(PagingFilterParams filterParams)
        {
            return await TryCatch(
                async () => await GetProductsInternal(filterParams),
                GenerateEmptyResult);
        }

        public async Task<ListResult<Product>> GetAllProductsRaw(ListFilter<Product> filter)
        {
            return await TryCatch(
                async () => await productService.ListAsync(filter),
                () => new ListResult<Product>(Enumerable.Empty<Product>(), null));
        }

        public async Task<ListResult<Product>> GetAllProductsRaw()
        {
            return await TryCatch(
                async () => await productService.ListAsync(),
                () => new ListResult<Product>(Enumerable.Empty<Product>(), null));
        }

        public async Task<Dictionary<string, ProductVariantListModel>> GetProductVariants(string shopifyProductID, string currencyCode)
        {
            return await TryCatch(
                async () => await GetProductVariantsInternal(shopifyProductID, currencyCode),
                () => []);
        }

        private async Task<ListResultWrapper<ShopifyProductListModel>> GetProductsInternal(PagingFilterParams filterParams)
        {
            var filter = new ListFilter<Product>(filterParams?.PageInfo, filterParams?.Limit, ShopifyFields);
            var result = await productService.ListAsync(filter, true);
            return CreateResultModel(result, settingsService.GetWebsiteChannelSettings()?.CurrencyCode ?? string.Empty);
        }

        private async Task<Dictionary<string, ProductVariantListModel>> GetProductVariantsInternal(string shopifyProductID, string currencyCode)
        {
            var cart = await shoppingService.GetCurrentShoppingCart();
            var variants = await GetVariantsFromApi(shopifyProductID, currencyCode);
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

        private async Task<ListResultWrapper<ShopifyProductListModel>> GetProductsAsyncInternal(ProductFilter initialFilter)
        {
            var filter = new ProductListFilter
            {
                Fields = ShopifyFields,
                CollectionId = initialFilter.CollectionID,
                Limit = initialFilter.Limit,
                Ids = initialFilter.Ids,
                PresentmentCurrencies = [initialFilter.Currency.ToString()]
            };
            var result = await productService.ListAsync(filter, true);

            return CreateResultModel(result, initialFilter.Currency.ToString());
        }

        private ListResultWrapper<ShopifyProductListModel> CreateResultModel(ListResult<Product> products, string? currency)
        {
            var items = new List<ShopifyProductListModel>();
            foreach (var item in products.Items)
            {
                var firstImage = item.Images.FirstOrDefault();
                (decimal? price, decimal? listPrice) = GetPrices(item.Variants, currency ?? defaultCurrency);
                items.Add(new ShopifyProductListModel
                {
                    Image = firstImage?.Src,
                    ImageAlt = firstImage?.Alt,
                    Name = item.Title,
                    Description = item.BodyHtml,
                    ShopifyUrl = $"{shopifyProductUrlBase.AbsoluteUri}/{Uri.EscapeDataString(item.Handle)}",
                    Price = price,
                    ListPrice = listPrice,
                    PriceFormatted = price.FormatPrice(currency),
                    ListPriceFormatted = listPrice.FormatPrice(currency),
                    HasMoreVariants = item.Variants.Count() > 1
                });
            }

            var nextPage = products.GetNextPageFilter();
            var prevPage = products.GetPreviousPageFilter();

            return new ListResultWrapper<ShopifyProductListModel>()
            {
                Items = items,
                PrevPageFilter = new PagingFilterParams()
                {
                    PageInfo = prevPage?.PageInfo,
                    Limit = prevPage?.Limit
                },
                NextPageFilter = new PagingFilterParams()
                {
                    PageInfo = nextPage?.PageInfo,
                    Limit = nextPage?.Limit
                }
            };
        }

        private (decimal? price, decimal? listPrice) GetPrices(IEnumerable<ProductVariant> variants, string? currency)
        {
            if (variants == null || !variants.Any())
            {
                return (null, null);
            }
            if (variants.Count() == 1)
            {
                var onlyVariant = variants.First();
                var currencyPrice = onlyVariant.PresentmentPrices?.FirstOrDefault(x => x.Price.CurrencyCode.Equals(currency, StringComparison.Ordinal));

                return currencyPrice is { Price: not null } ?
                    (currencyPrice.Price.Amount, currencyPrice.CompareAtPrice?.Amount) : (null, null);
            }

            decimal? minPrice = null;

            foreach (var variant in variants)
            {
                var currencyPrice = variant.PresentmentPrices?.FirstOrDefault(x => x.Price.CurrencyCode.Equals(currency, StringComparison.Ordinal));

                if (currencyPrice?.Price.Amount != null)
                {
                    decimal price = currencyPrice.Price.Amount.Value;

                    if (!minPrice.HasValue || price < minPrice.Value)
                    {
                        minPrice = price;
                    }
                }
            }

            return (minPrice, null);
        }

        private ListResultWrapper<ShopifyProductListModel> GenerateEmptyResult()
        {
            return new ListResultWrapper<ShopifyProductListModel>()
            {
                Items = Enumerable.Empty<ShopifyProductListModel>()
            };
        }

        private async Task<Dictionary<string, ProductVariantListModel>> GetVariantsFromApi(string shopifyProductID, string currencyCode)
        {
            if (!long.TryParse(shopifyProductID, out long productIdValue))
            {
                return [];
            }

            var filter = new ProductListFilter()
            {
                PresentmentCurrencies = [currencyCode],
                Ids = [productIdValue],
                Fields = "variants"
            };

            var variants = (await productService.ListAsync(filter, true)).Items.First().Variants;
            var inventoryItems = await inventoryService.GetVariantsInventoryItems(variants.Select(x => x.InventoryItemId ?? 0));
            return variants.ToDictionary(x => x.Id?.ToString() ?? string.Empty, x => GetVariantListModel(x, currencyCode, inventoryItems));
        }

        private ProductVariantListModel GetVariantListModel(ProductVariant variant, string currencyCode, Dictionary<long, InventoryItem> inventoryItems)
        {
            var prices = variant.PresentmentPrices.First();
            bool onStock = true;
            if (inventoryItems.TryGetValue(variant.InventoryItemId ?? 0, out var inventoryItem))
            {
                onStock = !(inventoryItem.Tracked ?? false) || variant.InventoryQuantity > 0;
            }
            return new ProductVariantListModel()
            {
                PriceFormatted = prices.Price.Amount.FormatPrice(currencyCode),
                ListPriceFormatted = prices.CompareAtPrice?.Amount.FormatPrice(currencyCode),
                StockCssClass = onStock ? "available" : "unavailable",
                StockStatusText = onStock ? "On stock" : "Out of stock",
                Available = onStock,
                MerchandiseID = variant.AdminGraphQLAPIId,
            };
        }
    }
}
