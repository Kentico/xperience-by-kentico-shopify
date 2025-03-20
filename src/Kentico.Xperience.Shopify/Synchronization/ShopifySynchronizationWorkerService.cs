﻿using CMS.ContentEngine;
using CMS.ContentEngine.Internal;
using CMS.Core;
using CMS.Membership;

using Kentico.Xperience.Shopify.Config;
using Kentico.Xperience.Shopify.Products;
using Kentico.Xperience.Shopify.ReusableContentTypes;
using Kentico.Xperience.Shopify.Synchronization.Images;
using Kentico.Xperience.Shopify.Synchronization.Products;
using Kentico.Xperience.Shopify.Synchronization.Variants;

using Microsoft.Extensions.Logging;

using Shopify;
using Shopify.ContentTypes;

namespace Kentico.Xperience.Shopify.Synchronization;

internal class ShopifySynchronizationWorkerService : IShopifySynchronizationWorkerService
{
    private readonly ILogger<ShopifySynchronizationWorkerService> logger;
    private readonly IShopifyIntegrationSettingsService shopifyIntegrationSettingsService;
    private readonly IShopifyProductService shopifyProductService;
    private readonly IShopifyContentItemService shopifyContentItemService;
    private readonly IImageSynchronizationService imageSynchronizationService;
    private readonly IVariantSynchronizationService variantSynchronizationService;
    private readonly IProductSynchronizationService productSynchronizationService;
    private readonly IShopifySynchronizationSettingsService synchronizationSettingsService;
    private readonly IContentFolderManager contentFolderManager;
    private readonly IContentQueryExecutor queryExecutor;

    public ShopifySynchronizationWorkerService(
        ILogger<ShopifySynchronizationWorkerService> logger,
        IShopifyIntegrationSettingsService shopifyIntegrationSettingsService,
        IShopifyProductService shopifyProductService,
        IShopifyContentItemService shopifyContentItemService,
        IImageSynchronizationService imageSynchronizationService,
        IVariantSynchronizationService variantSynchronizationService,
        IProductSynchronizationService productSynchronizationService,
        IShopifySynchronizationSettingsService synchronizationSettingsService,
        IContentFolderManagerFactory contentFolderManagerFactory,
        IContentQueryExecutor queryExecutor)
    {
        contentFolderManager = contentFolderManagerFactory.Create(UserInfoProvider.AdministratorUser.UserID);

        this.logger = logger;
        this.shopifyIntegrationSettingsService = shopifyIntegrationSettingsService;
        this.shopifyProductService = shopifyProductService;
        this.shopifyContentItemService = shopifyContentItemService;
        this.imageSynchronizationService = imageSynchronizationService;
        this.variantSynchronizationService = variantSynchronizationService;
        this.productSynchronizationService = productSynchronizationService;
        this.synchronizationSettingsService = synchronizationSettingsService;
        this.queryExecutor = queryExecutor;
    }

    public async Task SynchronizeProducts()
    {

        var integrationSettings = shopifyIntegrationSettingsService.GetSettings();
        if (integrationSettings == null)
        {
            logger.LogError("Cannot start products synchronization. No Shopify integration settings provided.");
            return;
        }

        if (!Uri.TryCreate(integrationSettings.ShopifyUrl, UriKind.Absolute, out var _))
        {
            logger.LogError("Cannot start products synchronization. Shopify store URL '{ShopifyUrl}' has incorrect format", integrationSettings.ShopifyUrl);
            return;
        }

        string languageName = "en";
        int adminUserID = UserInfoProvider.AdministratorUser.UserID;

        var shopifyProducts = await shopifyProductService.GetAllProductsRaw();
        var contentItemProducts = await shopifyContentItemService.GetContentItems<ShopifyProductItem>(Product.CONTENT_TYPE_NAME, 2);

        await productSynchronizationService.DeleteNonExistingProducts(contentItemProducts, shopifyProducts, languageName, adminUserID);

        var contentItemProductsLookup = contentItemProducts.ToLookup(x => x.ShopifyProductID);
        var syncSettings = await synchronizationSettingsService.GetSettings();

        var createdImages = new List<int>();
        var createdProducts = new List<int>();
        var createdVariants = new List<int>();

        foreach (var shopifyProduct in shopifyProducts)
        {
            var productContentItem = contentItemProductsLookup[shopifyProduct.Id].FirstOrDefault();
            var imageContentItems = GetAllProductImages(productContentItem);

            var imageSyncResult = await imageSynchronizationService.ProcessImages(
                shopifyProduct,
                imageContentItems,
                languageName,
                syncSettings.WorkspaceName,
                adminUserID);

            var variantSyncResult = await variantSynchronizationService.ProcessVariants(
                shopifyProduct.Variants,
                productContentItem?.Variants,
                imageSyncResult.VariantImages,
                languageName,
                syncSettings.WorkspaceName,
                adminUserID);

            var productSyncResult = await productSynchronizationService.ProcessProduct(
                shopifyProduct,
                variantSyncResult.ProductVariantGuids,
                imageSyncResult.ProductImages,
                languageName,
                syncSettings.WorkspaceName,
                adminUserID,
                productContentItem);

            createdImages.AddRange(imageSyncResult.CreatedImages);
            createdVariants.AddRange(variantSyncResult.CreatedVariantContentItemIDs);
            if (productSyncResult.NewProductCreated)
            {
                createdProducts.Add(productSyncResult.ProductContentItemID);
            }
        }

        await MoveItemsToDestFolder(syncSettings.ProductFolder.FolderID, createdProducts);
        await MoveItemsToDestFolder(syncSettings.ProductVariantFolder.FolderID, createdVariants);
        await MoveItemsToDestFolder(syncSettings.ImageFolder.FolderID, createdImages);

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

    private async Task MoveItemsToDestFolder(int targetFolderId, IEnumerable<int> ids)
    {
        if (targetFolderId == 0)
        {
            return;
        }

        if (ids.Any())
        {
            await contentFolderManager.MoveItems(targetFolderId, ids);
        }
    }
}
