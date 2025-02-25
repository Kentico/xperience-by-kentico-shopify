using Microsoft.AspNetCore.Mvc.Rendering;

namespace DancingGoat.Models;

public record ProductDetailViewModel
{
    public string ProductName { get; init; }

    public IEnumerable<ImageViewModel> Images { get; init; } = [];

    public string DescriptionHTML { get; init; }

    public string ParametersSection { get; init; }

    public IEnumerable<SelectListItem> Variants { get; init; } = [];

    public string SelectedShopifyVariantId { get; init; }

    public string ShopifyProductId { get; init; }

    public string CountryCode { get; init; }

    public int VariantQuantity { get; init; }

    public string SelectedVariantMerchandiseID { get; init; }

    public string[] ErrorMessages { get; init; }

    public static ProductDetailViewModel GetViewModel(ProductDetailPage page, string selectedVariantID, string country, string[] errorMessages)
    {
        var product = page.Product.First();
        var selectedVariant = product.Variants.FirstOrDefault(x => x.ShopifyVariantID.Equals(selectedVariantID, StringComparison.Ordinal)) ?? product.Variants.First();

        var allImages = product.Images.Concat(product.Variants.Select(x => x.Image.FirstOrDefault()))
            .Where(x => x != null)
            .Select(x => new ImageViewModel
            {
                ImageUrl = x.ImageAsset.Url,
                Alt = x.ImageDescription,
                Title = product.Title
            });

        return new ProductDetailViewModel
        {
            ProductName = product.Title,
            Images = allImages,
            DescriptionHTML = product.Description,
            ParametersSection = product.Parameters,
            Variants = product.Variants.Select(x => new SelectListItem(x.Title, x.ShopifyVariantID, x.ShopifyVariantID.Equals(selectedVariant.ShopifyVariantID, StringComparison.Ordinal))).ToList(),
            SelectedShopifyVariantId = selectedVariant.ShopifyVariantID,
            ShopifyProductId = product.ID,
            CountryCode = country,
            VariantQuantity = 1,
            SelectedVariantMerchandiseID = selectedVariant.ShopifyMerchandiseID,
            ErrorMessages = errorMessages
        };
    }
}
