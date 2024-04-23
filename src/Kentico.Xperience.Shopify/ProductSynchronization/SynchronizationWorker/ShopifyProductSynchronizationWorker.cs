﻿using System.Diagnostics;
using CMS.Base;
using CMS.Core;
using CMS.Membership;
using Kentico.Xperience.Shopify.ReusableContentTypes;
using Kentico.Xperience.Shopify.Services.ProductService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shopify;
using Shopify.ContentTypes;

namespace Kentico.Xperience.Shopify.ProductSynchronization;
internal class ShopifyProductSynchronizationWorker : ThreadWorker<ShopifyProductSynchronizationWorker>
{
    protected override int DefaultInterval => _defaultInterval;

    private ILogger<ShopifyProductSynchronizationWorker> logger = null!;
    private readonly int _defaultInterval = Convert.ToInt32(TimeSpan.FromMinutes(15).TotalMilliseconds);

    protected override void Initialize()
    {
        base.Initialize();
        logger = Service.Resolve<ILogger<ShopifyProductSynchronizationWorker>>();
    }

    /// <summary>Method processing actions.</summary>
    protected override void Process()
    {
        Debug.WriteLine($"Worker {GetType().FullName} running");

        try
        {
            SynchronizeProducts();
            logger.LogInformation("Shopify product synchronization done.");
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Error occured during running '{GetType().Name}'");
        }
    }

    private void SynchronizeProducts()
    {
        using var serviceScope = Service.Resolve<IServiceProvider>().CreateScope();
        var provider = serviceScope.ServiceProvider;
        var productService = provider.GetRequiredService<IShopifyProductService>();
        var contentItemService = provider.GetRequiredService<IShopifyContentItemService>();
        var imageUploaderService = provider.GetRequiredService<IImageSynchronizationService>();
        var variantSynchronizationService = provider.GetRequiredService<IVariantSynchronizationService>();
        var productSynchronizationService = provider.GetRequiredService<IProductSynchronizationService>();

        string languageName = "en";
        int adminUserID = UserInfoProvider.AdministratorUser.UserID;

        var shopifyProducts = productService.GetAllProductsRaw().GetAwaiter().GetResult().Items;
        var contentItemProducts = contentItemService.GetContentItems<ShopifyProductItem>(Product.CONTENT_TYPE_NAME, 2)
            .GetAwaiter().GetResult();

        productSynchronizationService.DeleteNonExistingProducts(contentItemProducts, shopifyProducts, languageName, adminUserID)
            .GetAwaiter().GetResult();

        var contentItemProductsLookup = contentItemProducts.ToLookup(x => x.ShopifyProductID);

        foreach (var shopifyProduct in shopifyProducts)
        {
            if (!shopifyProduct.Id.HasValue)
            {
                continue;
            }
            var productContentItem = contentItemProductsLookup[shopifyProduct.Id.Value.ToString()].FirstOrDefault();
            var imageContentItems = GetAllProductImages(productContentItem);

            var imageSyncResult = imageUploaderService.ProcessImages(shopifyProduct.Images, imageContentItems, languageName, adminUserID)
                .GetAwaiter().GetResult();
            var variantGuids = variantSynchronizationService.ProcessVariants(shopifyProduct.Variants, productContentItem?.Variants, imageSyncResult.VariantImages, languageName, adminUserID)
                .GetAwaiter().GetResult();
            productSynchronizationService.ProcessProduct(shopifyProduct, variantGuids, imageSyncResult.ProductImages, languageName, adminUserID, productContentItem)
                .GetAwaiter().GetResult();
        }

        logger.LogInformation("Finished shopify product synchronization.");
    }

    private IEnumerable<ShopifyImageItem> GetAllProductImages(ShopifyProductItem? product)
    {
        if (product == null)
        {
            return Enumerable.Empty<ShopifyImageItem>();
        }

        var images = new List<ShopifyImageItem>();

        if (product.Images != null && product.Images.Any())
        {
            images.AddRange(product.Images);
        }
        if (product.Variants == null || !product.Variants.Any())
        {
            return images;
        }

        foreach (var variant in product.Variants.Where(variant => variant.Image != null && variant.Image.Any()))
        {
            images.AddRange(variant.Image);
        }

        return images;
    }

    protected override void Finish()
    {
    }
}
