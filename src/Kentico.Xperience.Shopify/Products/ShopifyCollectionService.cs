using Kentico.Xperience.Shopify.Config;
using Kentico.Xperience.Shopify.Products.Models;

using ShopifySharp;
using ShopifySharp.Factories;
using ShopifySharp.Filters;

namespace Kentico.Xperience.Shopify.Products
{
    internal class ShopifyCollectionService : ShopifyServiceBase, IShopifyCollectionService
    {
        private readonly IGraphService graphService;

        public ShopifyCollectionService(IShopifyIntegrationSettingsService integrationSettingsService,
            IGraphServiceFactory graphServiceFactory) : base(integrationSettingsService)
        {
            graphService = graphServiceFactory.Create(shopifyCredentials);
        }

        public async Task<IEnumerable<CollectionListingModel>> GetCollectionListing()
        {
            return await TryCatch(GetCollectionListingInternal, Enumerable.Empty<CollectionListingModel>);
        }

        private async Task<IEnumerable<CollectionListingModel>> GetCollectionListingInternal()
        {
            var request = new GraphRequest
            {
                Query = "query CustomCollectionList { collections(first: 250) { nodes { id title } } }"
            };

            var modelList = new List<CollectionListingModel>();

            var result = await graphService.PostAsync<ProductCollectionsResult>(request);

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

