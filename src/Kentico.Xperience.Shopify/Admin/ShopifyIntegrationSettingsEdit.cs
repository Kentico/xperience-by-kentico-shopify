using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.Forms;
using Kentico.Xperience.Shopify.Admin;
using Kentico.Xperience.Shopify.Config;

[assembly: UIPage(
    parentType: typeof(ShopifyIntegrationSettingsApplication),
    slug: "shopify-settings-edit",
    uiPageType: typeof(ShopifyIntegrationSettingsEdit),
    name: "Shopify settings",
    templateName: TemplateNames.EDIT,
    order: UIPageOrder.First)]

namespace Kentico.Xperience.Shopify.Admin
{
    /// <summary>
    /// Page for editing Shopify integration settings.
    /// </summary>
    public class ShopifyIntegrationSettingsEdit : ModelEditPage<ShopifyIntegrationSettingsModel>
    {
        private readonly IShopifyIntegrationSettingsService settingsService;
        /// <summary>
        /// Initializes a new instance of the <see cref="ShopifyIntegrationSettingsEdit"/> class.
        /// </summary>
        public ShopifyIntegrationSettingsEdit(
            Xperience.Admin.Base.Forms.Internal.IFormItemCollectionProvider formItemCollectionProvider,
            IFormDataBinder formDataBinder,
            IShopifyIntegrationSettingsService settingsService) : base(formItemCollectionProvider, formDataBinder)
        {
            this.settingsService = settingsService;
        }

        private IntegrationSettingsInfo? settingsInfo;
        private IntegrationSettingsInfo? SettingsInfo => settingsInfo ??= IntegrationSettingsInfo.Provider.Get()
            .TopN(1)
            .FirstOrDefault();
        private ShopifyIntegrationSettingsModel? model;


        /// <summary>
        /// Gets or sets the model for Shopify integration settings.
        /// </summary>
        protected override ShopifyIntegrationSettingsModel Model => model ??= CreateShopifySettingsModel(SettingsInfo);


        /// <inheritdoc/>
        public override Task ConfigurePage()
        {

            PageConfiguration.Headline = "It is recommended to use appsettings.json or user secrets to store API credentials. Use this primarly for developement. Values in appsettings.json/user secrets will override these values.";

            if (!settingsService.AdminUISettingsUsed())
            {
                PageConfiguration.SubmitConfiguration.ConfirmationConfiguration = new ConfirmationConfiguration()
                {
                    Title = "Warning",
                    Detail = "These settings are overridden by values from appsettings.json or user secrets. Changes won't affect the application.",
                    Button = "Save",
                };
            }

            return base.ConfigurePage();
        }


        /// <inheritdoc/>
        protected override Task<ICommandResponse> ProcessFormData(ShopifyIntegrationSettingsModel model, ICollection<IFormItem> formItems)
        {
            var info = SettingsInfo ?? new IntegrationSettingsInfo();

            info.ShopifyUrl = model.ShopifyStoreUrl;
            info.AdminApiKey = model.AdminApiKey;
            info.StorefrontApiKey = model.StorefrontApiKey;
            info.StorefrontApiVersion = model.StorefrontApiVersion;

            IntegrationSettingsInfo.Provider.Set(info);

            return base.ProcessFormData(model, formItems);
        }


        private ShopifyIntegrationSettingsModel CreateShopifySettingsModel(IntegrationSettingsInfo? integrationSettings)
        {
            if (integrationSettings == null)
            {
                return new ShopifyIntegrationSettingsModel();
            }

            return new ShopifyIntegrationSettingsModel()
            {
                AdminApiKey = integrationSettings.AdminApiKey,
                StorefrontApiKey = integrationSettings.StorefrontApiKey,
                ShopifyStoreUrl = integrationSettings.ShopifyUrl,
                StorefrontApiVersion = integrationSettings.StorefrontApiVersion,
            };
        }
    }
}
