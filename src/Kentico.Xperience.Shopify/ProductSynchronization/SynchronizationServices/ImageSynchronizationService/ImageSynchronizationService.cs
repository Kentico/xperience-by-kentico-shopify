﻿using CMS.ContentEngine;
using CMS.Core;
using Kentico.Xperience.Ecommerce.Common.ContentItemSynchronization;
using Kentico.Xperience.Shopify.ReusableContentTypes;
using ShopifySharp;

namespace Kentico.Xperience.Shopify.ProductSynchronization;
public class ImageSynchronizationService : SynchronizationServiceBase, IImageSynchronizationService
{
    private readonly IHttpClientFactory httpClientFactory;

    public ImageSynchronizationService(IShopifyContentItemService contentItemService, IEventLogService eventLogService, IHttpClientFactory httpClientFactory)
        : base(contentItemService, eventLogService)
    {
        this.httpClientFactory = httpClientFactory;
    }

    public async Task<ImageSynchronizationResult> ProcessImages(
        IEnumerable<ProductImage> shopifyImages,
        IEnumerable<ShopifyImageItem>? imagesCI,
        string languageName,
        int userID)
    {
        var resultImages = new List<ShopifyImageItem>();

        var shopifyImageIDs = shopifyImages.Select(x => x.Id?.ToString());
        var imagesContentItemIDs = imagesCI?.Select(x => x.ShopifyImageID) ?? Enumerable.Empty<string>();

        var nonExistingIDs = shopifyImageIDs.Where(x => imagesCI == null || !imagesContentItemIDs.Contains(x));
        var existingIDs = shopifyImageIDs.Except(nonExistingIDs);

        var syncResult = new ImageSynchronizationResult();

        // Remove old images
        var removedIDs = await DeleteNonExistingItems(imagesCI, shopifyImages, languageName, userID);
        if (imagesCI != null && imagesCI.Any())
        {
            resultImages.AddRange(imagesCI.Where(x => !removedIDs.Contains(x.SystemFields.ContentItemID)));
        }

        // Create new content item images
        var newImages = shopifyImages.Where(x => x.Id.HasValue && nonExistingIDs.Contains(x.Id?.ToString()));
        if (newImages.Any())
        {
            resultImages.AddRange(await CreateNewImages(newImages, languageName, userID, syncResult));
        }

        // Update existing content item images
        var shopifyImagesLookup = shopifyImages.ToLookup(x => x.Id?.ToString() ?? string.Empty);
        var imagesCILookup = imagesCI?.ToLookup(x => x.ShopifyImageID);
        await UpdateExistingImages(existingIDs, shopifyImagesLookup, imagesCILookup, languageName, userID, syncResult);

        syncResult.ProductImages = OrderItemsByShopify(resultImages, shopifyImages.Where(x => !x.VariantIds?.Any() ?? true).OrderBy(x => x.Position)).ToList();

        return syncResult;
    }

    private async Task<bool> UpdateImageContentItem(ShopifyImageItem productImageCI, string newImageUrl, string languageName, int userID)
    {
        var syncModel = new ImageSynchronizationItem()
        {
            ImageName = productImageCI.ImageName,
            ImageAsset = await CreateAssetMetadata(newImageUrl, productImageCI.ImageName),
            ImageDescription = productImageCI.ImageDescription,
            ShopifyImageID = productImageCI.ShopifyImageID
        };

        return await contentItemService.UpdateContentItem(new ContentItemUpdateParams()
        {
            ContentItemParams = syncModel.ToDict(),
            ContentItemID = productImageCI.SystemFields.ContentItemID,
            LanguageName = languageName,
            UserID = userID,
            VersionStatus = productImageCI.SystemFields.ContentItemCommonDataVersionStatus
        });
    }

    /// <summary>
    /// Get content items from DB and create <see cref="ImageSynchronizationResult" />
    /// </summary>
    /// <param name="contentItemKvp">
    /// Dictionary where key value is content item ID and value is list of Shopify product variantIDs if any exist.
    /// If value is null or empty, content item with given ID belongs to Shopify product.
    /// </param>
    /// <param name="contentItems">Image content items.</param>
    /// <returns></returns>
    private ImageSynchronizationResult GetSyncResult(Dictionary<int, IEnumerable<string>?> contentItemKvp, IEnumerable<ShopifyImageItem> contentItems)
    {
        var syncResult = new ImageSynchronizationResult();
        foreach (var systemFields in contentItems.Select(x => x.SystemFields))
        {
            var imageGuid = systemFields.ContentItemGUID;
            if (contentItemKvp.TryGetValue(systemFields.ContentItemID, out var variantIDs) && variantIDs != null && variantIDs.Any())
            {
                foreach (string? variant in variantIDs)
                {
                    syncResult.VariantImages.TryAdd(variant, imageGuid);
                }
            }
            else
            {
                syncResult.ProductImages.Add(imageGuid);
            }
        }

        return syncResult;
    }

    /// <summary>
    /// Create new <see cref="ShopifyImageItem"/> items in content hub and appends them into <paramref name="syncResult"/>.
    /// </summary>
    /// <param name="shopifyImages"></param>
    /// <param name="languageName"></param>
    /// <param name="userID"></param>
    /// <param name="syncResult"></param>
    /// <returns>Created content items.</returns>
    private async Task<IEnumerable<ShopifyImageItem>> CreateNewImages(IEnumerable<ProductImage> shopifyImages, string languageName, int userID, ImageSynchronizationResult syncResult)
    {
        var uploadModels = shopifyImages.Select(x => new ImageUploadModel()
        {
            ImageName = NameFromUrl(x.Src),
            ImageUrl = x.Src,
            Description = x.Alt,
            ShopifyImageID = x.Id?.ToString() ?? string.Empty,
            VariantIDs = x.VariantIds?.Select(x => x.ToString())
        });

        // Upload images to content hub and create dictionary where key is Content item ID and value is list of Shopify variant IDs
        var uploadedKvp = uploadModels.ToDictionary(x => UploadProductImage(x, languageName, userID).GetAwaiter().GetResult(), x => x.VariantIDs);

        var imageContentItems = await contentItemService.GetContentItems<ShopifyImageItem>(global::Shopify.Image.CONTENT_TYPE_NAME,
                config => config.Where(x => x.WhereIn(nameof(ShopifyImageItem.SystemFields.ContentItemID), uploadedKvp.Keys.ToArray()))
                    .Columns(nameof(ShopifyImageItem.SystemFields.ContentItemGUID), nameof(ShopifyImageItem.SystemFields.ContentItemID), nameof(ShopifyImageItem.ShopifyImageID)));

        var localSyncResult = GetSyncResult(uploadedKvp, imageContentItems);

        syncResult.ProductImages.AddRange(localSyncResult.ProductImages.Except(syncResult.ProductImages));
        foreach (var localVariant in localSyncResult.VariantImages)
        {
            if (syncResult.VariantImages.ContainsKey(localVariant.Key))
            {
                syncResult.VariantImages[localVariant.Key] = localVariant.Value;
            }
            else
            {
                syncResult.VariantImages.TryAdd(localVariant.Key, localVariant.Value);
            }
        }

        return imageContentItems;
    }

    private async Task UpdateExistingImages(
        IEnumerable<string?> shopifyImageIDs,
        ILookup<string, ProductImage> shopifyImages,
        ILookup<string, ShopifyImageItem>? imagesCI,
        string languageName,
        int userID,
        ImageSynchronizationResult syncResult)
    {
        if (shopifyImageIDs == null || !shopifyImageIDs.Any())
        {
            return;
        }

        // Dictionary to store content item image GUID with list of shopify variant IDs that are using the image
        var contentItemDict = new Dictionary<Guid, IEnumerable<string>?>();

        foreach (string? shopifyImageID in shopifyImageIDs)
        {
            if (string.IsNullOrEmpty(shopifyImageID))
            {
                continue;
            }

            var shopifyImage = shopifyImages[shopifyImageID].FirstOrDefault();
            var contentItemImage = imagesCI?[shopifyImageID].FirstOrDefault();

            if (shopifyImage == null || contentItemImage == null)
            {
                continue;
            }

            var syncItem = new ImageSynchronizationItem()
            {
                ImageName = NameFromUrl(shopifyImage.Src),
                ImageDescription = shopifyImage.Alt ?? string.Empty,
                ShopifyImageID = shopifyImage.Id?.ToString() ?? string.Empty
            };

            if (syncItem.GetModifiedProperties(contentItemImage, out var modifiedProps))
            {
                if (modifiedProps.ContainsKey(nameof(ShopifyImageItem.ShopifyImageID)))
                {
                    // Needs to update image asset as well
                    await UpdateImageContentItem(contentItemImage, shopifyImage.Src, languageName, userID);
                }
                else
                {
                    // No need to update image asset. ContentItemService can be used
                    await contentItemService.UpdateContentItem(new ContentItemUpdateParams()
                    {
                        ContentItemParams = modifiedProps,
                        ContentItemID = contentItemImage.SystemFields.ContentItemID,
                        LanguageName = languageName,
                        UserID = userID,
                        VersionStatus = contentItemImage.SystemFields.ContentItemCommonDataVersionStatus
                    });
                }
            }
            contentItemDict.TryAdd(contentItemImage.SystemFields.ContentItemGUID, shopifyImage.VariantIds?.Select(x => x.ToString()));
        }

        foreach (var kvp in contentItemDict)
        {
            if (kvp.Value != null && kvp.Value.Any())
            {
                foreach (string shopifyVariantID in kvp.Value)
                {
                    if (syncResult.VariantImages.ContainsKey(shopifyVariantID))
                    {
                        syncResult.VariantImages[shopifyVariantID] = kvp.Key;
                    }
                    else
                    {
                        syncResult.VariantImages.TryAdd(shopifyVariantID, kvp.Key);
                    }
                }
            }
            else
            {
                syncResult.ProductImages.Add(kvp.Key);
            }
        }
    }

    private async Task<int> UploadProductImage(ImageUploadModel uploadModel, string languageName, int userID)
    {
        var addParams = new ContentItemAddParams()
        {
            ContentItem = new ImageSynchronizationItem()
            {
                ImageName = uploadModel.ImageName,
                ImageAsset = await CreateAssetMetadata(uploadModel.ImageUrl, uploadModel.ImageName),
                ImageDescription = uploadModel.Description,
                ShopifyImageID = uploadModel.ShopifyImageID
            },
            LanguageName = languageName,
            UserID = userID
        };

        return await contentItemService.AddContentItem(addParams);
    }

    private async Task<ContentItemAssetMetadataWithSource> CreateAssetMetadata(string url, string name)
    {
        byte[] bytes;
        using (var client = httpClientFactory.CreateClient())
        {
            bytes = await client.GetByteArrayAsync(url);
        }

        long length = bytes.LongLength;
        var dataWrapper = new BinaryDataWrapper(bytes);
        var fileSource = new ContentItemAssetStreamSource((CancellationToken cancellationToken) => Task.FromResult(dataWrapper.Stream));
        string extension = Path.GetExtension(name) ?? string.Empty;
        var assetMetadata = new ContentItemAssetMetadata()
        {
            Extension = extension,
            Identifier = Guid.NewGuid(),
            LastModified = DateTime.Now,
            Name = name,
            Size = length
        };

        return new ContentItemAssetMetadataWithSource(fileSource, assetMetadata);
    }

    private string NameFromUrl(string url) => new Uri(url).Segments.LastOrDefault()?.Split('?')[0] ?? string.Empty;
}
