using Microsoft.AspNetCore.Mvc;

namespace Kentico.Xperience.Shopify.Products.Api;

/// <summary>
/// Controller for handling Shopify product API requests.
/// </summary>
[Route("/api/shopify/product")]
public class ShopifyProductApiController : Controller
{
    private readonly IShopifyProductService productService;


    /// <summary>
    /// Initializes a new instance of the <see cref="ShopifyProductApiController"/> class.
    /// </summary>
    /// <param name="productService">The Shopify product service.</param>
    public ShopifyProductApiController(IShopifyProductService productService)
    {
        this.productService = productService;
    }


    /// <summary>
    /// Retrieves product prices for the specified product ID and currency.
    /// </summary>
    /// <param name="productId">The ID of the product.</param>
    /// <param name="currency">The currency for which prices are retrieved.</param>
    /// <returns>The action result containing the product variants.</returns>
    [HttpGet]
    public async Task<IActionResult> GetProductPrices([FromQuery] string productId, [FromQuery] string currency)
    {
        var variants = await productService.GetProductVariants(productId, currency);
        return Json(variants);
    }
}

