namespace Shopify
{
    public partial class Product
    {
        /// <summary>
        /// Short product identifier.
        /// Short identifier contains only the number part.
        /// </summary>
        public string ProductIDShort => ShopifyProductID.Split('/')[^1];
    }
}
