namespace Kentico.Xperience.Shopify.ShoppingCart;

public interface IShoppingService
{
    Task<CartOperationResult> UpdateCartItem(ShoppingCartItemParameters parameters);

    Task<CartOperationResult> RemoveCartItem(string merchandiseId);

    Task<ShoppingCartInfo?> GetCurrentShoppingCart();

    Task<CartOperationResult> AddItemToCart(ShoppingCartItemParameters parameters);
}

