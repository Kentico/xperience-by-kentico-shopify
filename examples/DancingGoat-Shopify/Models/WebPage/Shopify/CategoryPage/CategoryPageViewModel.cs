using CMS.Websites;

using Kentico.Xperience.Shopify.Products.Models;

namespace DancingGoat.Models;

public record CategoryPageViewModel
{
    public string CategoryName { get; init; }
    public IEnumerable<ShopifyProductListItemViewModel> Products { get; init; }

    public static CategoryPageViewModel GetViewModel(
        CategoryPage category,
        IDictionary<string, ProductPriceModel> productPrices,
        IEnumerable<ProductDetailPage> products,
        IDictionary<Guid, WebPageUrl> productUrls,
        ILogger logger,
        string currencyCode)
    {
        var productListItems = new List<ShopifyProductListItemViewModel>();
        foreach (var product in products)
        {
            if (product.Product == null || !product.Product.Any())
            {
                logger.LogWarning($"Product page {product.SystemFields.WebPageItemName} does not contain any product content item");
            }
            else
            {
                productListItems.Add(GetProductListItem(product, productUrls, productPrices, currencyCode));
            }
        }
        var model = new CategoryPageViewModel
        {
            CategoryName = category.CategoryName,
            Products = productListItems
        };

        return model;
    }

    private static ShopifyProductListItemViewModel GetProductListItem(
        ProductDetailPage productPage,
        IDictionary<Guid, WebPageUrl> productUrls,
        IDictionary<string, ProductPriceModel> productPrices,
        string currencyCode)
    {
        var product = productPage.Product.FirstOrDefault();
        if (product == null)
        {
            return new ShopifyProductListItemViewModel();
        }

        productUrls.TryGetValue(productPage.SystemFields.WebPageItemGUID, out var url);
        productPrices.TryGetValue(product.ProductIDShort, out var productPriceModel);
        return ShopifyProductListItemViewModel.GetViewModel(product, url, productPriceModel, currencyCode);
    }
}
