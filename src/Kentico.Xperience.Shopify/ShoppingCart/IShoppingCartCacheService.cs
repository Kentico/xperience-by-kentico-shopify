namespace Kentico.Xperience.Shopify.ShoppingCart
{
    internal interface IShoppingCartCacheService
    {
        void UpdateCartCache(ShoppingCartInfo cart);

        Task<ShoppingCartInfo?> LoadAsync(string cartId, Func<string, Task<ShoppingCartInfo?>> retriveCartFunc);
    }
}
