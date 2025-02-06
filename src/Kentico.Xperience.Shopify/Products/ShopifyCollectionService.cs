using CMS.Helpers;

using Kentico.Xperience.Shopify.Config;
using Kentico.Xperience.Shopify.Products.Models;

using Microsoft.Extensions.Logging;

using ShopifySharp;
using ShopifySharp.Factories;
using ShopifySharp.GraphQL;
using ShopifySharp.Services.Graph;

namespace Kentico.Xperience.Shopify.Products
{
    internal class ShopifyCollectionService : ShopifyServiceBase, IShopifyCollectionService
    {
        private readonly IGraphService graphService;

        private const CurrencyCode defaultCurrency = CurrencyCode.USD;

        public ShopifyCollectionService(IShopifyIntegrationSettingsService integrationSettingsService,
            IGraphServiceFactory graphServiceFactory) : base(integrationSettingsService)
        {
            graphService = graphServiceFactory.Create(shopifyCredentials);
        }

        public async Task<IEnumerable<CollectionListingModel>> GetCollectionListing()
            => await TryCatch(GetCollectionListingInternal, Enumerable.Empty<CollectionListingModel>);

        public async Task<IEnumerable<ShopifyProductListModel>> GetCollectionProducts(string collectionId, int topN, CountryCode countryCode)
        {
            GraphResult<CollectionResult>? result = null;

            var query = new GraphRequest
            {
                Query = "query getCollectionById($id:ID!,$topN:Int,$country:CountryCode){collection(id:$id){title products(first:$topN){nodes{contextualPricing(context:{country:$country}){minVariantPricing{price{amount currencyCode}compareAtPrice{amount currencyCode}}}featuredMedia{...on MediaImage{__typename image{altText url}}}title description onlineStorePreviewUrl}}}}",
                Variables = new Dictionary<string, object>()
                {
                    ["id"] = collectionId,
                    ["topN"] = topN,
                    ["country"] = countryCode.ToStringRepresentation()
                }
            };

            try
            {
                result = await graphService.PostAsync<CollectionResult>(query);
            }
            catch (ShopifyGraphErrorsException ex)
            {
                logger.LogError(ex, "Could not fetch collection products");
            }

            var products = result?.Data.Collection.products?.nodes;

            if (products is null)
            {
                return [];
            }

            var items = new List<ShopifyProductListModel>();

            foreach (var product in products)
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

            return items;
        }

        private async Task<IEnumerable<CollectionListingModel>> GetCollectionListingInternal()
        {
            var request = new GraphRequest
            {
                Query = "query CustomCollectionList { collections(first: 250) { nodes { id title } } }"
            };

            var modelList = new List<CollectionListingModel>();

            var result = await graphService.PostAsync<CollectionConnectionResult>(request);

            if (result.Data.Collections.nodes is null)
            {
                return modelList;
            }

            foreach (var collectionListing in result.Data.Collections.nodes)
            {
                modelList.Add(new CollectionListingModel
                {
                    CollectionID = collectionListing.id ?? string.Empty,
                    Name = collectionListing.title ?? string.Empty,
                });
            }

            return modelList;
        }
    }
}

