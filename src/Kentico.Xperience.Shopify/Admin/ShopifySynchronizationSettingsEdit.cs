using CMS.ContentEngine;
using CMS.Membership;

using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.Forms;
using Kentico.Xperience.Admin.Base.Forms.Internal;
using Kentico.Xperience.Ecommerce.Common.Admin;
using Kentico.Xperience.Shopify.Admin;

[assembly: UIPage(
    parentType: typeof(ShopifyIntegrationSettingsApplication),
    slug: "synchzonization-settings-edit",
    uiPageType: typeof(ShopifySynchronizationSettingsEdit),
    name: "Synchronization settings",
    templateName: TemplateNames.EDIT,
    order: 300)]
namespace Kentico.Xperience.Shopify.Admin
{
    internal class ShopifySynchronizationSettingsEdit : ModelEditPage<SynchronizationSettingsModel>
    {
        private readonly IContentFolderManager contentFolderManager;

        private SynchronizationSettingsInfo? settingsInfo;
        private SynchronizationSettingsModel? model;

        public SynchronizationSettingsInfo? SettingsInfo => settingsInfo ??= SynchronizationSettingsInfo.Provider.Get()
            .TopN(1)
            .FirstOrDefault();

        public ShopifySynchronizationSettingsEdit(
            IFormItemCollectionProvider formItemCollectionProvider,
            IFormDataBinder formDataBinder,
            IContentFolderManagerFactory contentFolderManagerFactory)
            : base(formItemCollectionProvider, formDataBinder)
        {
            contentFolderManager = contentFolderManagerFactory.Create(UserInfoProvider.AdministratorUser.UserID);
        }


        protected override SynchronizationSettingsModel Model => model ??= CreateSynchronizationSettingsModel(SettingsInfo);

        private SynchronizationSettingsModel CreateSynchronizationSettingsModel(SynchronizationSettingsInfo? settingsInfo)
        {
            var settingsModel = new SynchronizationSettingsModel();
            if (settingsInfo == null)
            {
                return settingsModel;
            }

            settingsModel.WorkspaceName = settingsInfo.ShopifyEffectiveWorkspaceName;

            var folderDict = GetFolderIdsByGuids(
                [settingsInfo.ShopifyProductFolderGuid, settingsInfo.ShopifyProductVariantFolderGuid, settingsInfo.ShopifyImageFolderGuid]);

            if (folderDict.TryGetValue(settingsInfo.ShopifyProductFolderGuid, out var productFolder))
            {
                settingsModel.ProductFolderId = productFolder;
            }
            if (folderDict.TryGetValue(settingsInfo.ShopifyProductVariantFolderGuid, out var variantFolder))
            {
                settingsModel.ProductVariantFolderId = variantFolder;
            }
            if (folderDict.TryGetValue(settingsInfo.ShopifyImageFolderGuid, out var imageFolder))
            {
                settingsModel.ImageFolderId = imageFolder;
            }

            return settingsModel;
        }

        protected override async Task<ICommandResponse> ProcessFormData(SynchronizationSettingsModel model, ICollection<IFormItem> formItems)
        {
            var rootFolder = (await contentFolderManager.GetRoot(model.WorkspaceName))?.ContentFolderID;
            var info = SettingsInfo ?? new SynchronizationSettingsInfo();
            info.ShopifyWorkspaceName = model.WorkspaceName;

            if (rootFolder != null)
            {
                AssignDefaultFolder(model, rootFolder.Value);
            }

            var folderDict = await GetFolderGuidsByIds([model.ProductFolderId, model.ProductVariantFolderId, model.ImageFolderId]);

            if (folderDict.TryGetValue(model.ProductFolderId, out var productFolder))
            {
                info.ShopifyProductFolderGuid = productFolder;
            }
            if (folderDict.TryGetValue(model.ProductVariantFolderId, out var variantFolder))
            {
                info.ShopifyProductVariantFolderGuid = variantFolder;
            }
            if (folderDict.TryGetValue(model.ImageFolderId, out var imageFolder))
            {
                info.ShopifyImageFolderGuid = imageFolder;
            }

            SynchronizationSettingsInfo.Provider.Set(info);

            return await base.ProcessFormData(model, formItems);
        }

        private async Task<Dictionary<int, Guid>> GetFolderGuidsByIds(IEnumerable<int> contentFolderIds)
        {
            var folders = await contentFolderManager.Get()
                .WhereIn(nameof(ContentFolderInfo.ContentFolderID), contentFolderIds)
                .Columns(nameof(ContentFolderInfo.ContentFolderID), nameof(ContentFolderInfo.ContentFolderGUID))
                .GetEnumerableTypedResultAsync();

            return folders.ToDictionary(x => x.ContentFolderID, x => x.ContentFolderGUID);
        }

        private Dictionary<Guid, int> GetFolderIdsByGuids(IEnumerable<Guid> contentFolderGuids)
        {
            var folders = contentFolderManager.Get()
                .WhereIn(nameof(ContentFolderInfo.ContentFolderGUID), contentFolderGuids)
                .Columns(nameof(ContentFolderInfo.ContentFolderID), nameof(ContentFolderInfo.ContentFolderGUID))
                .GetEnumerableTypedResult();

            return folders.ToDictionary(x => x.ContentFolderGUID, x => x.ContentFolderID);
        }

        private static void AssignDefaultFolder(SynchronizationSettingsModel model, int defaultFolder)
        {
            if (defaultFolder == 0)
            {
                return;
            }

            if (model.ProductFolderId == 0)
            {
                model.ProductFolderId = defaultFolder;
            }
            if (model.ProductVariantFolderId == 0)
            {
                model.ProductVariantFolderId = defaultFolder;
            }
            if (model.ImageFolderId == 0)
            {
                model.ImageFolderId = defaultFolder;
            }
        }
    }
}
