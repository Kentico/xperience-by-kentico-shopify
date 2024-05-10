namespace Kentico.Xperience.Shopify.ShoppingCart;

public class CartOperationResult
{
    public ShoppingCartInfo? Cart { get; set; }
    public bool Success { get; set; }
    public IEnumerable<string> ErrorMessages { get; set; }

    public CartOperationResult(ShoppingCartInfo? cart, bool success)
    {
        Cart = cart;
        Success = success;
        ErrorMessages = [];
    }

    public CartOperationResult(ShoppingCartInfo? cart, bool success, IEnumerable<string> errorMessages)
    {
        Cart = cart;
        Success = success;
        ErrorMessages = errorMessages;
    }
}
