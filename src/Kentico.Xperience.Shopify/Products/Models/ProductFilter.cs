using ShopifySharp.GraphQL;

namespace Kentico.Xperience.Shopify.Products.Models
{
    /// <summary>
    /// Filter for product retrieval.
    /// </summary>
    public class ProductFilter
    {
        /// <summary>
        /// The ID of the Shopify collection to filter by.
        /// </summary>
        public long? CollectionID { get; set; }


        /// <summary>
        /// The currency to use for pricing.
        /// </summary>
        public CountryCode Country { get; set; }


        /// <summary>
        /// The maximum number of products to retrieve.
        /// </summary>
        public int? Limit { get; set; }


        /// <summary>
        /// The IDs of products to retrieve.
        /// </summary>
        public IEnumerable<long>? Ids { get; set; }


        /// <summary>
        /// Start cursor used for pagination.
        /// </summary>
        public string? StartCursor { get; set; }


        /// <summary>
        /// End cursor used for pagination.
        /// </summary>
        public string? EndCursor { get; set; }
    }
}
