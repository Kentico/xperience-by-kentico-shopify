using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Modules;

namespace Kentico.Xperience.Shopify.Admin;

internal interface IShopifyIntegrationSettingsModuleInstaller
{
    void Install();
}

internal class ShopifyIntegrationSettingsModuleInstaller : IShopifyIntegrationSettingsModuleInstaller
{
    private readonly IInfoProvider<ResourceInfo> resourceInfoProvider;
    private readonly IInfoProvider<IntegrationSettingsInfo> shopifySettingsInfoProvider;

    public ShopifyIntegrationSettingsModuleInstaller(IInfoProvider<ResourceInfo> resourceInfoProvider, IInfoProvider<IntegrationSettingsInfo> shopifySettingsInfoProvider)
    {
        this.resourceInfoProvider = resourceInfoProvider;
        this.shopifySettingsInfoProvider = shopifySettingsInfoProvider;
    }

    public void Install()
    {
        var resourceInfo = InstallModule();
        InstallSettingsInfo(resourceInfo);
    }

    private ResourceInfo InstallModule()
    {
        var resourceInfo = resourceInfoProvider.Get(ShopifyResourceConstants.ResourceName)
            ?? new ResourceInfo();

        resourceInfo.ResourceDisplayName = ShopifyResourceConstants.ResourceDisplayName;
        resourceInfo.ResourceName = ShopifyResourceConstants.ResourceName;
        resourceInfo.ResourceDescription = ShopifyResourceConstants.ResourceDescription;
        resourceInfo.ResourceIsInDevelopment = ShopifyResourceConstants.ResourceIsInDevelopment;
        if (resourceInfo.HasChanged)
        {
            resourceInfoProvider.Set(resourceInfo);
        }
        return resourceInfo;
    }

    private void InstallSettingsInfo(ResourceInfo resourceInfo)
    {
        var info = DataClassInfoProvider.GetDataClassInfo(IntegrationSettingsInfo.TYPEINFO.ObjectClassName) ??
            DataClassInfo.New(IntegrationSettingsInfo.OBJECT_TYPE);

        info.ClassName = IntegrationSettingsInfo.TYPEINFO.ObjectClassName;
        info.ClassTableName = IntegrationSettingsInfo.TYPEINFO.ObjectClassName.Replace(".", "_");
        info.ClassDisplayName = "Shopify Settings info";
        info.ClassResourceID = resourceInfo.ResourceID;
        info.ClassType = ClassType.OTHER;

        var formInfo = FormHelper.GetBasicFormDefinition(nameof(IntegrationSettingsInfo.IntegrationSettingsID));

        var formItem = new FormFieldInfo
        {
            Name = nameof(IntegrationSettingsInfo.ShopifyUrl),
            Visible = true,
            DataType = FieldDataType.Text,
            Caption = ShopifySettingsConstants.SettingsShopifyUrl,
            Enabled = true,
            AllowEmpty = false,
        };
        formInfo.AddFormItem(formItem);

        formItem = new FormFieldInfo
        {
            Name = nameof(IntegrationSettingsInfo.AdminApiKey),
            Visible = true,
            DataType = FieldDataType.Text,
            Caption = ShopifySettingsConstants.SettingsAdminApiKey,
            Enabled = true,
            AllowEmpty = true,
        };
        formInfo.AddFormItem(formItem);

        formItem = new FormFieldInfo
        {
            Name = nameof(IntegrationSettingsInfo.StorefrontApiKey),
            Visible = true,
            DataType = FieldDataType.Text,
            Caption = ShopifySettingsConstants.SettingsStorefrontApiKey,
            Enabled = true,
            AllowEmpty = true,
        };
        formInfo.AddFormItem(formItem);

        formItem = new FormFieldInfo
        {
            Name = nameof(IntegrationSettingsInfo.StorefrontApiVersion),
            Visible = true,
            DataType = FieldDataType.Text,
            Caption = ShopifySettingsConstants.SettingsStorefrontApiVersion,
            Enabled = true,
            AllowEmpty = true,
        };
        formInfo.AddFormItem(formItem);

        //formItem = new FormFieldInfo
        //{
        //    Name = nameof(IntegrationSettingsInfo.ShopifyWorkspaceName),
        //    Visible = true,
        //    DataType = FieldDataType.Text,
        //    Caption = ShopifySettingsConstants.SettingsWorkspaceName,
        //    Enabled = true,
        //    AllowEmpty = true,
        //};
        //formInfo.AddFormItem(formItem);

        //formItem = new FormFieldInfo
        //{
        //    Name = nameof(IntegrationSettingsInfo.ShopifyProductSKUFolderGuid),
        //    Visible = true,
        //    DataType = FieldDataType.Guid,
        //    Caption = ShopifySettingsConstants.SettingsProductSKUFolderGuid,
        //    Enabled = true,
        //    AllowEmpty = false,
        //};
        //formInfo.AddFormItem(formItem);

        //formItem = new FormFieldInfo
        //{
        //    Name = nameof(IntegrationSettingsInfo.ShopifyProductVariantFolderGuid),
        //    Visible = true,
        //    DataType = FieldDataType.Guid,
        //    Caption = ShopifySettingsConstants.SettingsProductVariantFolderGuid,
        //    Enabled = true,
        //    AllowEmpty = false,
        //};
        //formInfo.AddFormItem(formItem);

        //formItem = new FormFieldInfo
        //{
        //    Name = nameof(IntegrationSettingsInfo.ShopifyProductImageFolderGuid),
        //    Visible = true,
        //    DataType = FieldDataType.Guid,
        //    Caption = ShopifySettingsConstants.SettingsProductImageFolderGuid,
        //    Enabled = true,
        //    AllowEmpty = false,
        //};
        //formInfo.AddFormItem(formItem);

        SetFormDefinition(info, formInfo);

        if (info.HasChanged)
        {
            DataClassInfoProvider.SetDataClassInfo(info);
        }

        var settings = shopifySettingsInfoProvider.Get().TopN(1).GetEnumerableTypedResult().FirstOrDefault();
        if (settings is null)
        {
            settings = new IntegrationSettingsInfo()
            {
                ShopifyUrl = string.Empty,
                AdminApiKey = string.Empty,
                StorefrontApiKey = string.Empty,
                StorefrontApiVersion = string.Empty,
                //ShopifyWorkspaceName = string.Empty,
                //ShopifyProductSKUFolderGuid = Guid.Empty,
                //ShopifyProductVariantFolderGuid = Guid.Empty,
                //ShopifyProductImageFolderGuid = Guid.Empty,
            };
            settings.Insert();
        }
    }

    /// <summary>
    /// Ensure that the form is not upserted with any existing form
    /// </summary>
    /// <param name="info"></param>
    /// <param name="form"></param>
    private static void SetFormDefinition(DataClassInfo info, FormInfo form)
    {
        if (info.ClassID > 0)
        {
            var existingForm = new FormInfo(info.ClassFormDefinition);
            existingForm.CombineWithForm(form, new());
            info.ClassFormDefinition = existingForm.GetXmlDefinition();
        }
        else
        {
            info.ClassFormDefinition = form.GetXmlDefinition();
        }
    }
}
