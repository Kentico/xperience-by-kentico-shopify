﻿using Kentico.Xperience.Shopify.ShoppingCart.GraphQLModels;

namespace Kentico.Xperience.Shopify.ShoppingCart;

internal class CartObjectModel : IGraphQLObjectBase
{
    public static string? MutationObjectScheme => QueryObjectScheme + @", userErrors { code, field, message }";
    public static string? QueryObjectScheme => @"{ id, buyerIdentity { countryCode } discountCodes { applicable, code } totalQuantity, checkoutUrl, lines(first: 250) { edges { node { id, quantity, cost { totalAmount { amount, currencyCode } } merchandise { ... on ProductVariant { id, title, product { title } } } discountAllocations { discountedAmount { amount currencyCode } ... on CartCodeDiscountAllocation { code } } } } } cost { totalAmount { amount currencyCode } subtotalAmount { amount currencyCode } } }";
    public required string Id { get; set; }
    public int TotalQuantity { get; set; }
    public required string CheckoutUrl { get; set; }
    public required CartCost Cost { get; set; }
    public CartLines? Lines { get; set; }
    public IEnumerable<DiscountCode>? DiscountCodes { get; set; }
}

internal class CartCost
{
    public required PriceDto TotalAmount { get; set; }
    public PriceDto? SubtotalAmount { get; set; }
}

internal class CartLines
{
    public required IEnumerable<CartLineEdge> Edges { get; set; }
}

internal class CartLineEdge
{
    public required CartLineNode Node { get; set; }
}

internal class CartLineNode
{
    public required string Id { set; get; }
    public required int Quantity { get; set; }
    public required CartCost Cost { get; set; }
    public required Merchandise Merchandise { get; set; }
    public IEnumerable<CartDiscountAllocation>? DiscountAllocations { get; set; }
}

internal class Merchandise
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required VariantProduct Product { get; set; }
}

internal class CartDiscountAllocation
{
    public required PriceDto DiscountedAmount { get; set; }
    public string? Code { get; set; }
}


/// <summary>
/// Discount code entity.
/// </summary>
public class DiscountCode
{
    /// <summary>
    /// Indicates if the discount code is applicable.
    /// </summary>
    public bool Applicable { get; set; }

    /// <summary>
    /// Required discount code.
    /// </summary>
    public required string Code { get; set; }
}

