using System;
using System.Data;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using Kentico.Xperience.Shopify.Admin;

[assembly: RegisterObjectType(typeof(SynchronizationSettingsInfo), SynchronizationSettingsInfo.OBJECT_TYPE)]

namespace Kentico.Xperience.Shopify.Admin
{
    /// <summary>
    /// Data container class for <see cref="SynchronizationSettingsInfo"/>.
    /// </summary>
    public partial class SynchronizationSettingsInfo : AbstractInfo<SynchronizationSettingsInfo, IInfoProvider<SynchronizationSettingsInfo>>, IInfoWithId, IInfoWithName, IInfoWithGuid
    {
        /// <summary>
        /// Object type.
        /// </summary>
        public const string OBJECT_TYPE = "shopify.synchronizationsettings";


        /// <summary>
        /// Type information.
        /// </summary>
        public static readonly ObjectTypeInfo TYPEINFO = new ObjectTypeInfo(typeof(IInfoProvider<SynchronizationSettingsInfo>), OBJECT_TYPE, "Shopify.SynchronizationSettings", "SynchronizationSettingsID", null, "ShopifyProductFolderGuid", "ShopifyWorkspaceName", null, null, null, null)
        {
            TouchCacheDependencies = true,
        };


        /// <summary>
        /// Synchronization settings ID.
        /// </summary>
        [DatabaseField]
        public virtual int SynchronizationSettingsID
        {
            get => ValidationHelper.GetInteger(GetValue(nameof(SynchronizationSettingsID)), 0);
            set => SetValue(nameof(SynchronizationSettingsID), value);
        }


        /// <summary>
        /// Shopify workspace name.
        /// </summary>
        [DatabaseField]
        public virtual string ShopifyWorkspaceName
        {
            get => ValidationHelper.GetString(GetValue(nameof(ShopifyWorkspaceName)), String.Empty);
            set => SetValue(nameof(ShopifyWorkspaceName), value, String.Empty);
        }


        /// <summary>
        /// Shopify product folder guid.
        /// </summary>
        [DatabaseField]
        public virtual Guid ShopifyProductFolderGuid
        {
            get => ValidationHelper.GetGuid(GetValue(nameof(ShopifyProductFolderGuid)), Guid.Empty);
            set => SetValue(nameof(ShopifyProductFolderGuid), value, Guid.Empty);
        }


        /// <summary>
        /// Shopify product variant folder guid.
        /// </summary>
        [DatabaseField]
        public virtual Guid ShopifyProductVariantFolderGuid
        {
            get => ValidationHelper.GetGuid(GetValue(nameof(ShopifyProductVariantFolderGuid)), Guid.Empty);
            set => SetValue(nameof(ShopifyProductVariantFolderGuid), value, Guid.Empty);
        }


        /// <summary>
        /// Shopify image folder guid.
        /// </summary>
        [DatabaseField]
        public virtual Guid ShopifyImageFolderGuid
        {
            get => ValidationHelper.GetGuid(GetValue(nameof(ShopifyImageFolderGuid)), Guid.Empty);
            set => SetValue(nameof(ShopifyImageFolderGuid), value, Guid.Empty);
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
        /// Creates an empty instance of the <see cref="SynchronizationSettingsInfo"/> class.
        /// </summary>
        public SynchronizationSettingsInfo()
            : base(TYPEINFO)
        {
        }


        /// <summary>
        /// Creates a new instances of the <see cref="SynchronizationSettingsInfo"/> class from the given <see cref="DataRow"/>.
        /// </summary>
        /// <param name="dr">DataRow with the object data.</param>
        public SynchronizationSettingsInfo(DataRow dr)
            : base(TYPEINFO, dr)
        {
        }
    }
}
