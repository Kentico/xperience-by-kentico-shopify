namespace Kentico.Xperience.Shopify.ShoppingCart
{
    internal interface IShoppingCartCacheService
    {
        /// <summary>
        /// Store updated shopping cart object into cache.
        /// </summary>
        /// <param name="cart">Shopping cart object</param>
        void UpdateCartCache(ShoppingCartInfo cart);


        /// <summary>
        /// Get shopping cart from cache or execute <paramref name="retriveCartFunc"/> function
        /// and store result into cache.
        /// </summary>
        /// <param name="cartId">Shopping cart GraphQL ID</param>
        /// <param name="retriveCartFunc">Function to retrieve shopping cart.</param>
        /// <returns>Current shopping cart or NULL if no shopping cart with given ID exists.</returns>
        Task<ShoppingCartInfo?> LoadAsync(string cartId, Func<string, Task<ShoppingCartInfo?>> retriveCartFunc);


        /// <summary>
        /// Remove shopping cart from cache.
        /// </summary>
        /// <param name="cartId">Shopping cart GraphQL ID</param>
        void RemoveShoppingCartCache(string cartId);
    }
}
