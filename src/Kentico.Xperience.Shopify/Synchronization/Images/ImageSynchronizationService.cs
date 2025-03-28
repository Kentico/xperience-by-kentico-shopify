using CMS.ContentEngine;
using CMS.Core;

using Kentico.Xperience.Ecommerce.Common.ContentItemSynchronization;
using Kentico.Xperience.Shopify.ReusableContentTypes;
using Kentico.Xperience.Shopify.Synchronization.BulkOperations;

using ShopifySharp;

using Path = System.IO.Path;

namespace Kentico.Xperience.Shopify.Synchronization.Images;
internal class ImageSynchronizationService : SynchronizationServiceBase, IImageSynchronizationService
{
    private readonly IHttpClientFactory httpClientFactory;

    public ImageSynchronizationService(IShopifyContentItemService contentItemService, IEventLogService eventLogService, IHttpClientFactory httpClientFactory)
        : base(contentItemService, eventLogService)
    {
        this.httpClientFactory = httpClientFactory;
    }

    public async Task<ImageSynchronizationResult> ProcessImages(
        ShopifyProductDto shopifyProduct,
        IEnumerable<ShopifyImageItem>? imagesCI,
        string languageName,
        string workspaceName,
        int userID)
    {
        var shopifyImages = shopifyProduct.Variants.SelectMany(x => x.Images).Concat(shopifyProduct.Images);

        (var toCreate, var toUpdate, var toDelete) = ClassifyItems(shopifyImages, imagesCI ?? []);

        var resultImages = new List<ShopifyImageItem>();
        var syncResult = new ImageSynchronizationResult();

        // Remove old images
        await contentItemService.DeleteContentItems(toDelete.Select(x => x.ContentItemIdentifier), languageName, userID);

        // Create new content item images
        if (toCreate is not null && toCreate.Any())
        {
            resultImages.AddRange(await CreateNewImages(toCreate, languageName, workspaceName, userID, syncResult));
        }

        // Update existing content item images
        await UpdateExistingImages(toUpdate, languageName, userID, syncResult);
        if (toUpdate.Any())
        {
            resultImages.AddRange(toUpdate.Select(x => x.ContentItem));
        }

        syncResult.ProductImages = OrderItemsByShopify(resultImages, shopifyImages.Where(x => x.Parent is ShopifyProductDto)).ToList();

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
    /// <returns>Images that should be assigned to particular variants and products.</returns>
    private ImageSynchronizationResult GetSyncResult(Dictionary<int, ImageUploadModel> contentItemKvp, IEnumerable<ShopifyImageItem> contentItems)
    {
        var syncResult = new ImageSynchronizationResult();
        foreach (var systemFields in contentItems.Select(x => x.SystemFields))
        {
            var imageGuid = systemFields.ContentItemGUID;
            if (contentItemKvp.TryGetValue(systemFields.ContentItemID, out var uploadModel))
            {
                if (uploadModel.VariantIds.Length != 0)
                {
                    foreach (var variantId in uploadModel.VariantIds)
                    {
                        syncResult.VariantImages.TryAdd(variantId, imageGuid);
                    }
                }
                else
                {
                    syncResult.ProductImages.Add(imageGuid);
                }
            }
        }

        return syncResult;
    }

    /// <summary>
    /// Create new <see cref="ShopifyImageItem"/> items in content hub and appends them into <paramref name="syncResult"/>.
    /// </summary>
    /// <param name="shopifyImages">List of retrieved images from Shopify.</param>
    /// <param name="languageName">Content items language.</param>
    /// <param name="workspaceName">Workspace name for synchronized content items.</param>
    /// <param name="userID">User ID used to add content items.</param>
    /// <param name="syncResult">List of variant and product images.</param>
    /// <returns>Created content items.</returns>
    private async Task<IEnumerable<ShopifyImageItem>> CreateNewImages(IEnumerable<ShopifyMediaImageDto> shopifyImages, string languageName, string workspaceName, int userID, ImageSynchronizationResult syncResult)
    {
        List<ImageUploadModel> uploadModels = [];
        foreach (var grouping in shopifyImages.GroupBy(x => x.Id))
        {
            // Image is assigned to variants (this is needed because Shopify images are always assigned to both variants and products).
            var variantIDs = grouping.Where(x => x.Parent is not ShopifyProductDto)
                .Select(x => x.ParentId!)
                .ToArray();

            var dto = grouping.First();
            uploadModels.Add(new ImageUploadModel()
            {
                ImageName = NameFromUrl(dto.Image.url!),
                ImageUrl = dto.Image.url!,
                Description = dto.Image.altText ?? string.Empty,
                ShopifyImageID = dto.Id,
                VariantIds = variantIDs
            });
        }

        // Upload images to content hub and create dictionary where key is Content item ID and value is list of Shopify variant IDs
        var uploadedKvp = uploadModels.ToDictionary(x => UploadProductImage(x, languageName, workspaceName, userID).GetAwaiter().GetResult());

        // Store created image content item IDs so they can be moved to folder later
        syncResult.CreatedImages = uploadedKvp.Keys;

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
        IEnumerable<(ShopifyMediaImageDto ShopifyItem, ShopifyImageItem ContentItem)> imagesToUpdate,
        string languageName,
        int userID,
        ImageSynchronizationResult syncResult)
    {
        if (!imagesToUpdate.Any())
        {
            return;
        }

        // Dictionary to store content item image GUID with list of shopify variant IDs that are using the image
        var contentItemDict = new Dictionary<Guid, List<string>?>();

        foreach ((var shopifyImage, var contentItemImage) in imagesToUpdate)
        {
            if (shopifyImage?.Image.url is null || contentItemImage is null)
            {
                continue;
            }

            var syncItem = new ImageSynchronizationItem()
            {
                ImageName = NameFromUrl(shopifyImage.Image.url),
                ImageDescription = shopifyImage.Image.altText ?? string.Empty,
                ShopifyImageID = shopifyImage.Id
            };

            if (syncItem.GetModifiedProperties(contentItemImage, out var modifiedProps))
            {
                if (modifiedProps.ContainsKey(nameof(ShopifyImageItem.ShopifyImageID)))
                {
                    // Needs to update image asset as well
                    await UpdateImageContentItem(contentItemImage, shopifyImage.Image.url, languageName, userID);
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

            // if image's parent is ProductVariant and contentItemsDict already contains image content item, add variantID to list
            if (shopifyImage.Parent is ShopifyProductVariantDto
                && shopifyImage.ParentId is not null
                && !contentItemDict.TryAdd(contentItemImage.SystemFields.ContentItemGUID, [shopifyImage.ParentId]))
            {
                contentItemDict[contentItemImage.SystemFields.ContentItemGUID]!.Add(shopifyImage.ParentId);
            }
        }

        foreach (var kvp in contentItemDict)
        {
            if (kvp.Value is not null && kvp.Value.Any())
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

    private async Task<int> UploadProductImage(ImageUploadModel uploadModel, string languageName, string workspaceName, int userID)
    {
        var addParams = new ContentItemAddParams()
        {
            ContentItem = new ImageSynchronizationItem()
            {
                ImageName = uploadModel.ImageName,
                ImageAsset = await CreateAssetMetadata(uploadModel.ImageUrl, uploadModel.ImageName),
                ImageDescription = uploadModel.Description,
                ShopifyImageID = uploadModel.ShopifyImageID,
            },
            LanguageName = languageName,
            WorkspaceName = workspaceName,
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
        var fileSource = new ContentItemAssetStreamSource((cancellationToken) => Task.FromResult(dataWrapper.Stream));
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
