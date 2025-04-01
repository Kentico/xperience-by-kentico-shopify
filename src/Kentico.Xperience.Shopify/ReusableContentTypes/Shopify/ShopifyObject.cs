namespace Shopify
{
    /// <summary>
    /// Shopify object base class.
    /// </summary>
    public abstract class ShopifyObject
    {
        /// <summary>
        /// Object ID retrieved from Global ID.
        /// IDs described in <seealso href="https://shopify.dev/docs/api/usage/gids"/>.
        /// </summary>
        public abstract string ID { get; }
    }
}
