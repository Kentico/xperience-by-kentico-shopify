using System;
using System.Data;
using System.Runtime.Serialization;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using Kentico.Xperience.Shopify.Admin;

[assembly: RegisterObjectType(typeof(ShopifySettingsInfo), ShopifySettingsInfo.OBJECT_TYPE)]

namespace Kentico.Xperience.Shopify.Admin
{
    /// <summary>
    /// Data container class for <see cref="ShopifySettingsInfo"/>.
    /// </summary>
    public partial class ShopifySettingsInfo : AbstractInfo<ShopifySettingsInfo, IInfoProvider<ShopifySettingsInfo>>, IInfoWithId
    {
        /// <summary>
        /// Object type.
        /// </summary>
        public const string OBJECT_TYPE = "shopify.settings";


        /// <summary>
        /// Type information.
        /// </summary>
        public static readonly ObjectTypeInfo TYPEINFO = new ObjectTypeInfo(typeof(IInfoProvider<ShopifySettingsInfo>), OBJECT_TYPE, "Shopify.Settings", "ShopifySettingsID", null, null, null, null, null, null, null)
        {
            TouchCacheDependencies = true,
        };


        /// <summary>
        /// Integration settings ID.
        /// </summary>
        [DatabaseField]
        public virtual int ShopifySettingsID
        {
            get => ValidationHelper.GetInteger(GetValue(nameof(ShopifySettingsID)), 0);
            set => SetValue(nameof(ShopifySettingsID), value);
        }

        /// <summary>
        /// Shopify workspace name
        /// </summary>
        [DatabaseField]
        public virtual string ShopifyWorkspaceName
        {
            get => ValidationHelper.GetString(GetValue(nameof(ShopifyWorkspaceName)), String.Empty);
            set => SetValue(nameof(ShopifyWorkspaceName), value);
        }

        /// <summary>
        /// Shopify settings product SKU folder Guid.
        /// </summary>
        [DatabaseField]
        public virtual Guid ShopifyProductSKUFolderGuid
        {
            get => ValidationHelper.GetGuid(GetValue(nameof(ShopifyProductSKUFolderGuid)), Guid.Empty);
            set => SetValue(nameof(ShopifyProductSKUFolderGuid), value);
        }


        /// <summary>
        /// Shopify settings product variant folder Guid.
        /// </summary>
        [DatabaseField]
        public virtual Guid ShopifyProductVariantFolderGuid
        {
            get => ValidationHelper.GetGuid(GetValue(nameof(ShopifyProductVariantFolderGuid)), Guid.Empty);
            set => SetValue(nameof(ShopifyProductVariantFolderGuid), value);
        }


        /// <summary>
        /// Shopify settings product image folder Guid.
        /// </summary>
        [DatabaseField]
        public virtual Guid ShopifyProductImageFolderGuid
        {
            get => ValidationHelper.GetGuid(GetValue(nameof(ShopifyProductImageFolderGuid)), Guid.Empty);
            set => SetValue(nameof(ShopifyProductImageFolderGuid), value);
        }


        /// <summary>
        /// Deletes the object using appropriate provider.
        /// </summary>
        protected override void DeleteObject()
        {
            Provider.Delete(this);
        }


        /// <summary>
        /// Updates the object using appropriate provider.
        /// </summary>
        protected override void SetObject()
        {
            Provider.Set(this);
        }



        /// <summary>
        /// Creates an empty instance of the <see cref="ShopifySettingsInfo"/> class.
        /// </summary>
        public ShopifySettingsInfo()
            : base(TYPEINFO)
        {
        }


        /// <summary>
        /// Creates a new instances of the <see cref="ShopifySettingsInfo"/> class from the given <see cref="DataRow"/>.
        /// </summary>
        /// <param name="dr">DataRow with the object data.</param>
        public ShopifySettingsInfo(DataRow dr)
            : base(TYPEINFO, dr)
        {
        }
    }
}
