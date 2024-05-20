namespace Kentico.Xperience.Shopify.Products.Models
{
    /// <summary>
    /// Parameters used for Shopify pagination.
    /// </summary>
    public class PagingFilterParams
    {
        /// <summary>
        /// Maximum results.
        /// </summary>
        public int? Limit { get; set; }


        /// <summary>
        /// Page identifier.
        /// </summary>
        public string? PageInfo { get; set; }
    }
}
