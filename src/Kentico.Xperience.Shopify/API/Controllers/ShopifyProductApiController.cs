using Kentico.Xperience.Shopify.Services.ProductService;
using Microsoft.AspNetCore.Mvc;

namespace Kentico.Xperience.Shopify.Controllers;

[Route("/api/shopify/product")]
public class ShopifyProductApiController : Controller
{
    private readonly IShopifyProductService productService;

    public ShopifyProductApiController(IShopifyProductService productService)
    {
        this.productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProductPrices([FromQuery] string productId, [FromQuery] string currency)
    {
        var variants = await productService.GetProductVariants(productId, currency);
        return Json(variants);
    }
}

