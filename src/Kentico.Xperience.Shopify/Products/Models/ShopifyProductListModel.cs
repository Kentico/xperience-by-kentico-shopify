namespace Kentico.Xperience.Shopify.Products.Models
{
    public class ShopifyProductListModel
    {
        public string? Image { get; set; }
        public string? ImageAlt { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ShopifyUrl { get; set; }
        public decimal? Price { get; set; }
        public decimal? ListPrice { get; set; }
        public string? PriceFormatted { get; set; }
        public string? ListPriceFormatted { get; set; }
        public bool HasMoreVariants { get; set; }
    }
}