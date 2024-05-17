namespace Kentico.Xperience.Shopify.ShoppingCart;

/// <summary>
/// Represents the result of a cart operation.
/// </summary>
public class CartOperationResult
{
    /// <summary>
    /// Updated shopping cart.
    /// </summary>
    public ShoppingCartInfo? Cart { get; set; }


    /// <summary>
    /// A value indicating whether the operation was successful.
    /// </summary>
    public bool Success { get; set; }


    /// <summary>
    /// Any error messages associated with the operation.
    /// </summary>
    public IEnumerable<string> ErrorMessages { get; set; }


    /// <summary>
    /// Initializes a new instance of the <see cref="CartOperationResult"/> class with the specified cart and success status.
    /// </summary>
    /// <param name="cart">The updated shopping cart information.</param>
    /// <param name="success">A value indicating whether the operation was successful.</param>
    public CartOperationResult(ShoppingCartInfo? cart, bool success)
    {
        Cart = cart;
        Success = success;
        ErrorMessages = [];
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="CartOperationResult"/> class with the specified cart, success status, and error messages.
    /// </summary>
    /// <param name="cart">The updated shopping cart information.</param>
    /// <param name="success">A value indicating whether the operation was successful.</param>
    /// <param name="errorMessages">Any error messages associated with the operation.</param>
    public CartOperationResult(ShoppingCartInfo? cart, bool success, IEnumerable<string> errorMessages)
    {
        Cart = cart;
        Success = success;
        ErrorMessages = errorMessages;
    }
}
