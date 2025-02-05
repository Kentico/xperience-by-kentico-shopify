namespace Kentico.Xperience.Shopify.Products.Models
{
    /// <summary>
    /// Model for listing Shopify product collections.
    /// </summary>
    public class CollectionListingModel
    {
        /// <summary>
        /// The name of the product collection.
        /// </summary>
        public string Name { get; set; } = string.Empty;


        /// <summary>
        /// The ID of the product collection.
        /// </summary>
        public string CollectionID { get; set; } = string.Empty;
    }
}

