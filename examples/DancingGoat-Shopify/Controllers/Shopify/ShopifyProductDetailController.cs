using System.Text;

using DancingGoat;
using DancingGoat.Models;

using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.Xperience.Shopify.Products.Models;
using Kentico.Xperience.Shopify.ShoppingCart;

using Microsoft.AspNetCore.Mvc;

using Shopify.Controllers;

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
        if (updateCartModel.VariantQuantity <= 0)
        {
            TempData[ERROR_MESSAGES_KEY] = new string[] { $"Cannot add {updateCartModel.VariantQuantity} items to cart. Minimum quantity is 1." };
        }
        else
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
        }

        var sb = new StringBuilder($"{HttpContext.Request.Path.Value ?? "/"}");
        if (updateCartModel.SelectedVariant != 0)
        {
            sb.Append($"?variantID={updateCartModel.SelectedVariant}");
        }

        return Redirect(sb.ToString());
    }
}
