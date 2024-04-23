using Kentico.Xperience.Shopify.ShoppingCart;

namespace Kentico.Xperience.Shopify.Tests.Mocks
{
    internal class ShoppingCartCacheServiceMock : IShoppingCartCacheService
    {
        public Task<ShoppingCartInfo?> LoadAsync(string cartId, Func<string, Task<ShoppingCartInfo?>> retriveCartFunc)
            => retriveCartFunc(cartId);


        public void UpdateCartCache(ShoppingCartInfo cart)
        {
        }
    }
}
