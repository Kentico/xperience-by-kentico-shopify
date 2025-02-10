using Kentico.Xperience.Shopify.Config;

using Microsoft.AspNetCore.Mvc;

using ShopifySharp.GraphQL;

namespace Kentico.Xperience.Shopify.Products.Api;

/// <summary>
/// Controller for handling Shopify product API requests.
/// </summary>
[Route("/api/shopify/product")]
public class ShopifyProductApiController : Controller
{
    private readonly IShopifyProductService productService;
    private readonly IShopifyIntegrationSettingsService settingsService;


    /// <summary>
    /// Initializes a new instance of the <see cref="ShopifyProductApiController"/> class.
    /// </summary>
    /// <param name="productService">The Shopify product service.</param>
    /// <param name="settingsService">Shopify integration settings service.</param>
    public ShopifyProductApiController(IShopifyProductService productService, IShopifyIntegrationSettingsService settingsService)
    {
        this.productService = productService;
        this.settingsService = settingsService;
    }


    /// <summary>
    /// Retrieves product prices for the specified product ID and currency.
    /// </summary>
    /// <param name="productId">The ID of the product.</param>
    /// <returns>The action result containing the product variants.</returns>
    [HttpGet]
    public async Task<IActionResult> GetProductPrices([FromQuery] string productId)
    {
        var settings = settingsService.GetWebsiteChannelSettings();
        var variants = await productService.GetProductVariants(productId, settings?.Country ?? CountryCode.US);
        return Json(variants);
    }
}

