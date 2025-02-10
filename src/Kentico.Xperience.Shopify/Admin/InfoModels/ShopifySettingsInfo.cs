using CMS.Workspaces.Internal;

namespace Kentico.Xperience.Shopify.Admin
{
    public partial class ShopifySettingsInfo
    {
        static ShopifySettingsInfo()
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
