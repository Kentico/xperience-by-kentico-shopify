using CMS.ContentEngine;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;

using Kentico.Xperience.Shopify.Admin;

namespace Kentico.Xperience.Shopify.Config
{
    internal class ShopifySynchronizationSettingsService : IShopifySynchronizationSettingsService
    {
        private readonly IInfoProvider<SynchronizationSettingsInfo> infoProvider;
        private readonly IProgressiveCache progressiveCache;
        private readonly IContentFolderManager contentFolderManager;

        public ShopifySynchronizationSettingsService(
            IInfoProvider<SynchronizationSettingsInfo> infoProvider,
            IProgressiveCache progressiveCache,
            IContentFolderManagerFactory contentFolderManagerFactory)
        {
            contentFolderManager = contentFolderManagerFactory.Create(UserInfoProvider.AdministratorUser.UserID);
            this.infoProvider = infoProvider;
            this.progressiveCache = progressiveCache;
        }

        public async Task<SynchronizationConfig> GetSettings()
            => await progressiveCache.LoadAsync(
                async cs => await GetConfigModel(),
                new CacheSettings(120, $"{nameof(ShopifySynchronizationSettingsService)}|{nameof(SynchronizationSettingsInfo)}")
                {
                    CacheDependency = CacheHelper.GetCacheDependency(
                        [$"{SynchronizationSettingsInfo.OBJECT_TYPE}|all", $"{ContentFolderInfo.OBJECT_TYPE}|all"])
                });

        private async Task<SynchronizationConfig> GetConfigModel()
        {
            var settings = (await infoProvider.Get().TopN(1).GetEnumerableTypedResultAsync())
                .FirstOrDefault() ?? new SynchronizationSettingsInfo();

            var folders = (await contentFolderManager.Get()
                    .WhereIn(
                        nameof(ContentFolderInfo.ContentFolderGUID),
                        [settings.ShopifyImageFolderGuid, settings.ShopifyProductVariantFolderGuid, settings.ShopifyProductFolderGuid])
                    .Columns(nameof(ContentFolderInfo.ContentFolderName), nameof(ContentFolderInfo.ContentFolderID), nameof(ContentFolderInfo.ContentFolderGUID))
                    .GetEnumerableTypedResultAsync())
                .ToLookup(x => x.ContentFolderGUID);

            var productFolder = folders[settings.ShopifyProductFolderGuid].FirstOrDefault();
            var variantFolder = folders[settings.ShopifyProductVariantFolderGuid].FirstOrDefault();
            var imageFolder = folders[settings.ShopifyImageFolderGuid].FirstOrDefault();

            return new SynchronizationConfig()
            {
                WorkspaceName = settings.ShopifyEffectiveWorkspaceName,
                ProductFolder = new SynchronizationFolder()
                {
                    FolderID = productFolder?.ContentFolderID ?? 0,
                    FolderGuid = settings.ShopifyProductFolderGuid,
                    FolderCodeName = productFolder?.ContentFolderName ?? string.Empty
                },
                ProductVariantFolder = new SynchronizationFolder()
                {
                    FolderID = variantFolder?.ContentFolderID ?? 0,
                    FolderGuid = settings.ShopifyProductVariantFolderGuid,
                    FolderCodeName = variantFolder?.ContentFolderName ?? string.Empty
                },
                ImageFolder = new SynchronizationFolder()
                {
                    FolderID = imageFolder?.ContentFolderID ?? 0,
                    FolderGuid = settings.ShopifyImageFolderGuid,
                    FolderCodeName = imageFolder?.ContentFolderName ?? string.Empty
                }
            };
        }
    }
}
