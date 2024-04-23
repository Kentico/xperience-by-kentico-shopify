﻿namespace Kentico.Xperience.Shopify.ShoppingCart;

public interface IShoppingService
{
    /// <summary>
    /// Update shopping cart item.
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns><see cref="CartOperationResult"/> with updated shopping cart if operation was successful.</returns>
    Task<CartOperationResult> UpdateCartItem(ShoppingCartItemParameters parameters);


    /// <summary>
    /// Remove shopping cart item.
    /// </summary>
    /// <param name="merchandiseId">Shopify product variant ID</param>
    /// <returns><see cref="CartOperationResult"/> with updated shopping cart if operation was successful.</returns>
    Task<CartOperationResult> RemoveCartItem(string merchandiseId);


    /// <summary>
    /// Get current shopping cart by key stored in session or cookies.
    /// </summary>
    /// <returns>Existing shopping cart or null.</returns>
    Task<ShoppingCartInfo?> GetCurrentShoppingCart();


    /// <summary>
    /// Add item to current shopping cart. If assigned shopping cart does not exist, create new one and then add item.
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns><see cref="CartOperationResult"/> with updated shopping cart if operation was successful.</returns>
    Task<CartOperationResult> AddItemToCart(ShoppingCartItemParameters parameters);
}
