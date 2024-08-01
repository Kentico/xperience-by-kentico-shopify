using CMS.Websites;

using Kentico.Xperience.Shopify;
using Kentico.Xperience.Shopify.Products.Models;

using Shopify;

namespace DancingGoat.Models;

public record ShopifyProductListItemViewModel
{
    public string MainImageUrl { get; init; }
    public string ProductName { get; init; }
    public string ProductDetailUrl { get; init; }
    public string Price { get; init; }
    public string ListPrice { get; init; }
    public bool HasMultipleVariants { get; init; }

    public static ShopifyProductListItemViewModel GetViewModel(Product product, WebPageUrl productUrl, ProductPriceModel priceModel, string currencyCode)
    {
        string currency = currencyCode;
        var mainImage = product.Images.FirstOrDefault() ?? product.Variants.FirstOrDefault(x => x.Image.Any())?.Image.FirstOrDefault();
        return new ShopifyProductListItemViewModel()
        {
            MainImageUrl = mainImage?.ImageAsset.Url ?? string.Empty,
            ProductName = product.Title,
            ProductDetailUrl = productUrl?.RelativePath ?? string.Empty,
            Price = priceModel?.Price.FormatPrice(currency),
            ListPrice = priceModel?.ListPrice.FormatPrice(currency),
            HasMultipleVariants = priceModel?.HasMultipleVariants ?? false
        };
    }
}
