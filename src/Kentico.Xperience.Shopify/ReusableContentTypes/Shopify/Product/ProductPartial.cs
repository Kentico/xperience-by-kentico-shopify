namespace Shopify
{
    public partial class Product : ShopifyObject
    {
        /// <inheritdoc/>
        public override string ID => ShopifyProductID.Split('/')[^1];
    }
}
