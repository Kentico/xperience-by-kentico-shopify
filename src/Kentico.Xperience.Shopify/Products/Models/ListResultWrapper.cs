namespace Kentico.Xperience.Shopify.Products.Models
{
    /// <summary>
    /// Generic wrapper class for a list result.
    /// Wrapper is used for pagination.
    /// </summary>
    /// <typeparam name="T">The type of items in the list.</typeparam>
    public class ListResultWrapper<T>
    {
        /// <summary>
        /// Result items.
        /// </summary>
        public IEnumerable<T>? Items { get; set; }


        /// <summary>
        /// Start cursor.
        /// </summary>
        public string? StartCursor { get; set; }

        /// <summary>
        /// End cursor.
        /// </summary>
        public string? EndCursor { get; set; }
    }
}

