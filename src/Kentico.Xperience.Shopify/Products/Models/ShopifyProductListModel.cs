namespace Kentico.Xperience.Shopify.Products.Models
{
    /// <summary>
    /// Represents a model for displaying Shopify product information.
    /// </summary>
    public class ShopifyProductListModel
    {
        /// <summary>
        /// The URL of the product image.
        /// </summary>
        public string? Image { get; set; }


        /// <summary>
        /// The alternate text for the product image.
        /// </summary>
        public string? ImageAlt { get; set; }


        /// <summary>
        /// The name of the product.
        /// </summary>
        public string? Name { get; set; }


        /// <summary>
        /// The description of the product.
        /// </summary>
        public string? Description { get; set; }


        /// <summary>
        /// The Shopify URL of the product.
        /// </summary>
        public string? ShopifyUrl { get; set; }


        /// <summary>
        /// The price of the product.
        /// </summary>
        public decimal? Price { get; set; }


        /// <summary>
        /// The list price of the product.
        /// </summary>
        public decimal? ListPrice { get; set; }


        /// <summary>
        /// The formatted price of the product.
        /// </summary>
        public string? PriceFormatted { get; set; }


        /// <summary>
        /// The formatted list price of the product.
        /// </summary>
        public string? ListPriceFormatted { get; set; }


        /// <summary>
        /// Indicates if the product has more variants.
        /// </summary>
        public bool HasMoreVariants { get; set; }
    }
}
