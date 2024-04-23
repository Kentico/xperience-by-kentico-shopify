using Kentico.Xperience.Shopify.Models;

namespace Kentico.Xperience.Shopify.Services
{
    public interface IShopifyCollectionService
    {
        Task<IEnumerable<CollectionListingModel>> GetCollectionListing();
    }
}
