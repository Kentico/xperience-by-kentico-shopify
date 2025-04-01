using CMS.Workspaces.Internal;

namespace Kentico.Xperience.Shopify.Admin
{
    public partial class SynchronizationSettingsInfo
    {
        /// <summary>
        /// Code name of the form to edit <see cref="SynchronizationSettingsInfo"/> object.
        /// </summary>
        public const string UI_FORM_NAME = "ShopifySynchronizationSettingsForm";

        static SynchronizationSettingsInfo()
        {
            TYPEINFO.ContinuousIntegrationSettings.Enabled = true;
        }

        /// <summary>
        /// Gets set workspace name or default workspace name (see: https://docs.kentico.com/developers-and-admins/configuration/users/role-management/workspaces#default-workspace)
        /// </summary>
        public string ShopifyEffectiveWorkspaceName
        {
            get
            {
                if (string.IsNullOrEmpty(ShopifyWorkspaceName))
                {
                    return WorkspaceConstants.WORKSPACE_DEFAULT_CODE_NAME;
                }
                return ShopifyWorkspaceName;
            }
        }
    }
}
