using ShopifySharp.GraphQL;

namespace Kentico.Xperience.Shopify.Products.Models;

/// <summary>
/// Model used to update the shopping cart.
/// </summary>
public class UpdateCartModel
{
    /// <summary>
    /// Country code enum.
    /// </summary>
    public CountryCode CountryCode { get; init; }

    /// <summary>
    /// Cart operation enum.
    /// </summary>
    public CartOperation CartOperation { get; set; }


    /// <summary>
    /// Shopify product variant ID to be updated.
    /// </summary>
    public long SelectedVariant { get; init; }


    /// <summary>
    /// Quantity that will be added/removed from shopping cart.
    /// </summary>
    public int VariantQuantity { get; init; }


    /// <summary>
    /// Shopify product variant GraphQL ID.
    /// </summary>
    public string? SelectedVariantMerchandiseID { get; init; }
}


/// <summary>
/// Represents the possible operations that can be performed on a shopping cart.
/// </summary>
public enum CartOperation
{
    /// <summary>
    /// Update an existing item in the cart.
    /// </summary>
    Update,

    /// <summary>
    /// Remove an item from the cart.
    /// </summary>
    Remove
}

