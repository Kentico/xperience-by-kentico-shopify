using CMS.Websites;
using Kentico.Xperience.Shopify.Models;

namespace DancingGoat.Models;

public record CategoryPageViewModel
{
    public string CategoryName { get; init; }
    public IEnumerable<ProductListItemViewModel> Products { get; init; }

    public static CategoryPageViewModel GetViewModel(
        CategoryPage category,
        IDictionary<string, ProductPriceModel> productPrices,
        IEnumerable<ProductDetailPage> products,
        IDictionary<Guid, WebPageUrl> productUrls,
        ILogger logger)
    {
        var productListItems = new List<ProductListItemViewModel>();
        foreach (var product in products)
        {
            if (product.Product == null || !product.Product.Any())
            {
                logger.LogWarning($"Product page {product.SystemFields.WebPageItemName} does not contain any product content item");
            }
            else
            {
                productListItems.Add(GetProductListItem(product, productUrls, productPrices));
            }
        }
        var model = new CategoryPageViewModel
        {
            CategoryName = category.CategoryName,
            Products = productListItems
        };

        return model;
    }

    private static ProductListItemViewModel GetProductListItem(ProductDetailPage productPage, IDictionary<Guid, WebPageUrl> productUrls, IDictionary<string, ProductPriceModel> productPrices)
    {
        var product = productPage.Product.FirstOrDefault();
        if (product == null)
        {
            return new ProductListItemViewModel();
        }

        productUrls.TryGetValue(productPage.SystemFields.WebPageItemGUID, out var url);
        productPrices.TryGetValue(product.ShopifyProductID, out var productPriceModel);
        return ProductListItemViewModel.GetViewModel(product, url, productPriceModel);
    }
}
