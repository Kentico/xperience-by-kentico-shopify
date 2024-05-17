namespace Kentico.Xperience.Shopify.Products.Models
{
    /// <summary>
    /// Generic wrapper class for a list result.
    /// </summary>
    /// <typeparam name="T">The type of items in the list.</typeparam>
    public class ListResultWrapper<T>
    {
        /// <summary>
        /// Result items.
        /// </summary>
        public IEnumerable<T>? Items { get; set; }


        /// <summary>
        /// Parameters to retrieve previous page.
        /// </summary>
        public PagingFilterParams? PrevPageFilter { get; set; }


        /// <summary>
        /// Parameters to retrieve next page.
        /// </summary>
        public PagingFilterParams? NextPageFilter { get; set; }
    }
}

