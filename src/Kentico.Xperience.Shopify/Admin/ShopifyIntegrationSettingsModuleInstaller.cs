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

    public ShopifyIntegrationSettingsModuleInstaller(IInfoProvider<ResourceInfo> resourceInfoProvider)
    {
        this.resourceInfoProvider = resourceInfoProvider;
    }

    public void Install()
    {
        var resourceInfo = InstallModule();
        InstallSettingsInfo(resourceInfo);
        InstallSynchronizationSettingsInfo(resourceInfo);
        InstallCurrencyFormatInfo(resourceInfo);
    }

    private void InstallCurrencyFormatInfo(ResourceInfo resourceInfo)
    {
        var info = DataClassInfoProvider.GetDataClassInfo(CurrencyFormatInfo.TYPEINFO.ObjectClassName) ??
    DataClassInfo.New(CurrencyFormatInfo.OBJECT_TYPE);

        info.ClassName = CurrencyFormatInfo.TYPEINFO.ObjectClassName;
        info.ClassTableName = CurrencyFormatInfo.TYPEINFO.ObjectClassName.Replace(".", "_");
        info.ClassDisplayName = "Shopify currency format";
        info.ClassResourceID = resourceInfo.ResourceID;
        info.ClassType = ClassType.OTHER;

        var formInfo = FormHelper.GetBasicFormDefinition(nameof(CurrencyFormatInfo.CurrencyFormatID));

        var formItem = new FormFieldInfo
        {
            Name = nameof(CurrencyFormatInfo.CurrencyCode),
            Visible = true,
            DataType = FieldDataType.Text,
            Caption = ShopifySettingsConstants.SettingsCurrencyCode,
            Enabled = true,
            AllowEmpty = false,
        };
        formInfo.AddFormItem(formItem);

        formItem = new FormFieldInfo
        {
            Name = nameof(CurrencyFormatInfo.CurrencyPriceFormat),
            Visible = true,
            DataType = FieldDataType.Text,
            Caption = ShopifySettingsConstants.SettingsCurrencyPriceFormat,
            Enabled = true,
            AllowEmpty = false,
        };
        formInfo.AddFormItem(formItem);

        SetFormDefinition(info, formInfo);

        if (info.HasChanged)
        {
            DataClassInfoProvider.SetDataClassInfo(info);
        }
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

        SetFormDefinition(info, formInfo);

        if (info.HasChanged)
        {
            DataClassInfoProvider.SetDataClassInfo(info);
        }
    }

    private void InstallSynchronizationSettingsInfo(ResourceInfo resourceInfo)
    {
        var info = DataClassInfoProvider.GetDataClassInfo(SynchronizationSettingsInfo.TYPEINFO.ObjectClassName) ??
            DataClassInfo.New(SynchronizationSettingsInfo.OBJECT_TYPE);

        info.ClassName = SynchronizationSettingsInfo.TYPEINFO.ObjectClassName;
        info.ClassTableName = SynchronizationSettingsInfo.TYPEINFO.ObjectClassName.Replace(".", "_");
        info.ClassDisplayName = "Shopify Synchronization settings";
        info.ClassResourceID = resourceInfo.ResourceID;
        info.ClassType = ClassType.OTHER;

        var formInfo = FormHelper.GetBasicFormDefinition(nameof(SynchronizationSettingsInfo.SynchronizationSettingsID));

        var formItem = new FormFieldInfo
        {
            Name = nameof(SynchronizationSettingsInfo.ShopifyWorkspaceName),
            Visible = true,
            DataType = FieldDataType.Text,
            Caption = ShopifySettingsConstants.SettingsWorkspaceName,
            Enabled = true,
            AllowEmpty = true,
        };
        formInfo.AddFormItem(formItem);

        formItem = new FormFieldInfo
        {
            Name = nameof(SynchronizationSettingsInfo.ShopifyProductFolderGuid),
            Visible = true,
            DataType = FieldDataType.Guid,
            Caption = ShopifySettingsConstants.SettingsProductSKUFolderGuid,
            Enabled = true,
            AllowEmpty = false,
        };
        formInfo.AddFormItem(formItem);

        formItem = new FormFieldInfo
        {
            Name = nameof(SynchronizationSettingsInfo.ShopifyProductVariantFolderGuid),
            Visible = true,
            DataType = FieldDataType.Guid,
            Caption = ShopifySettingsConstants.SettingsProductVariantFolderGuid,
            Enabled = true,
            AllowEmpty = false,
        };
        formInfo.AddFormItem(formItem);

        formItem = new FormFieldInfo
        {
            Name = nameof(SynchronizationSettingsInfo.ShopifyImageFolderGuid),
            Visible = true,
            DataType = FieldDataType.Guid,
            Caption = ShopifySettingsConstants.SettingsProductImageFolderGuid,
            Enabled = true,
            AllowEmpty = false,
        };
        formInfo.AddFormItem(formItem);

        SetFormDefinition(info, formInfo);

        if (info.HasChanged)
        {
            DataClassInfoProvider.SetDataClassInfo(info);
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
