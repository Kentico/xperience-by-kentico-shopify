using DancingGoat.Models;
using DancingGoat;
using Kentico.Content.Web.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using Kentico.Content.Web.Mvc;
using Shopify.Controllers;
using Kentico.Xperience.Shopify.ShoppingCart;
using Kentico.Xperience.Shopify.Products.Models;

[assembly: RegisterWebPageRoute(ProductDetailPage.CONTENT_TYPE_NAME, typeof(ShopifyProductDetailController), WebsiteChannelNames = new[] { DancingGoatConstants.WEBSITE_CHANNEL_NAME })]


namespace Shopify.Controllers;

public class ShopifyProductDetailController : Controller
{
    private const string ERROR_MESSAGES_KEY = "ErrorMessages";

    private readonly ProductDetailPageRepository productDetailPageRepository;
    private readonly IWebPageDataContextRetriever webPageDataContextRetriever;
    private readonly IShoppingService shoppingService;

    public ShopifyProductDetailController(ProductDetailPageRepository productDetailPageRepository,
        IWebPageDataContextRetriever webPageDataContextRetriever,
        IShoppingService shoppingService)
    {
        this.productDetailPageRepository = productDetailPageRepository;
        this.webPageDataContextRetriever = webPageDataContextRetriever;
        this.shoppingService = shoppingService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string variantID = null)
    {
        // TODO - dynamic resolve country
        string country = "CZ";
        string currency = "CZK";

        if (!TempData.TryGetValue(ERROR_MESSAGES_KEY, out object tempDataErrors) || tempDataErrors is not string[] errorMessages)
        {
            errorMessages = [];
        }

        var webPage = webPageDataContextRetriever.Retrieve().WebPage;
        var productDetail = await productDetailPageRepository.GetProductDetailPage(webPage.WebPageItemID, webPage.LanguageName, HttpContext.RequestAborted);

        if (productDetail.Product == null || !productDetail.Product.Any())
        {
            return View(new ProductDetailViewModel());
        }

        return View(ProductDetailViewModel.GetViewModel(productDetail, variantID ?? string.Empty, country, currency, errorMessages));
    }

    [HttpPost]
    public async Task<IActionResult> Index(UpdateCartModel updateCartModel, CartOperation cartOperation)
    {
        var cartItemParams = new ShoppingCartItemParameters()
        {
            Quantity = updateCartModel.VariantQuantity,
            MerchandiseID = updateCartModel.SelectedVariantMerchandiseID,
            Country = updateCartModel.CountryCode
        };

        var result = cartOperation == CartOperation.Remove
            ? await shoppingService.RemoveCartItem(cartItemParams.MerchandiseID)
            : await shoppingService.AddItemToCart(cartItemParams);

        TempData[ERROR_MESSAGES_KEY] = result.ErrorMessages.ToArray();

        return Redirect($"{HttpContext.Request.Path.Value ?? "/"}?variantID={updateCartModel.SelectedVariant}");
    }
}
