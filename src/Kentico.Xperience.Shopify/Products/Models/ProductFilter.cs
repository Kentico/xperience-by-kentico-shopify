using ShopifySharp.GraphQL;

namespace Kentico.Xperience.Shopify.Products.Models
{
    public class ProductFilter
    {
        public long? CollectionID { get; set; }
        public CurrencyCode? Currency { get; set; }
        public int? Limit { get; set; }
        public IEnumerable<long>? Ids { get; set; }
    }
}
