using Kentico.Xperience.Shopify.Config;
using Kentico.Xperience.Shopify.Models;
using Microsoft.Extensions.Options;
using ShopifySharp;
using ShopifySharp.Factories;
using ShopifySharp.Filters;

namespace Kentico.Xperience.Shopify.Services
{
    internal class ShopifyCollectionService : ShopifyServiceBase, IShopifyCollectionService
    {
        private readonly ICustomCollectionService customCollectionService;
        private readonly ISmartCollectionService smartCollectionService;

        public ShopifyCollectionService(IOptionsMonitor<ShopifyConfig> options,
            ICustomCollectionServiceFactory customCollectionServiceFactory,
            ISmartCollectionServiceFactory smartCollectionServiceFactory) : base(options)
        {
            customCollectionService = customCollectionServiceFactory.Create(shopifyCredentials);
            smartCollectionService = smartCollectionServiceFactory.Create(shopifyCredentials);
        }

        public async Task<IEnumerable<CollectionListingModel>> GetCollectionListing()
        {
            return await TryCatch(GetCollectionListingInternal, Enumerable.Empty<CollectionListingModel>);
        }

        private ListFilter<T> GenerateListFilter<T>()
        {
            return new ListFilter<T>(string.Empty, null, fields: "id,title");
        }

        private async Task<IEnumerable<CollectionListingModel>> GetCollectionListingInternal()
        {
            var modelList = new List<CollectionListingModel>();

            var customCollectionsTask = customCollectionService.ListAsync(GenerateListFilter<CustomCollection>());
            var smartCollectionsTask = smartCollectionService.ListAsync(GenerateListFilter<SmartCollection>());

            // Requests can run parallel
            await Task.WhenAll(customCollectionsTask, smartCollectionsTask);

            var customCollections = customCollectionsTask.Result;
            var smartCollections = smartCollectionsTask.Result;

            foreach (var collectionListing in customCollections.Items.Where(x => x != null && x.Id.HasValue))
            {
                modelList.Add(new CollectionListingModel
                {
                    CollectionID = collectionListing.Id.GetValueOrDefault(),
                    Name = collectionListing.Title ?? "",
                });
            }
            foreach (var collectionListing in smartCollections.Items.Where(x => x != null && x.Id.HasValue))
            {
                modelList.Add(new CollectionListingModel
                {
                    CollectionID = collectionListing.Id.GetValueOrDefault(),
                    Name = collectionListing.Title ?? "",
                });
            }

            return modelList;
        }
    }
}

