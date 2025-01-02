using System;
using System.Data;
using System.Runtime.Serialization;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using Kentico.Xperience.Shopify.Admin;

[assembly: RegisterObjectType(typeof(IntegrationSettingsInfo), IntegrationSettingsInfo.OBJECT_TYPE)]

namespace Kentico.Xperience.Shopify.Admin
{
    /// <summary>
    /// Data container class for <see cref="IntegrationSettingsInfo"/>.
    /// </summary>
    [Serializable]
    public partial class IntegrationSettingsInfo : AbstractInfo<IntegrationSettingsInfo, IInfoProvider<IntegrationSettingsInfo>>, IInfoWithId
    {
        /// <summary>
        /// Object type.
        /// </summary>
        public const string OBJECT_TYPE = "shopify.integrationsettings";


        /// <summary>
        /// Type information.
        /// </summary>
        public static readonly ObjectTypeInfo TYPEINFO = new ObjectTypeInfo(typeof(IInfoProvider<IntegrationSettingsInfo>), OBJECT_TYPE, "Shopify.IntegrationSettings", "IntegrationSettingsID", null, null, null, null, null, null, null)
        {
            TouchCacheDependencies = true,
        };


        /// <summary>
        /// Integration settings ID.
        /// </summary>
        [DatabaseField]
        public virtual int IntegrationSettingsID
        {
            get => ValidationHelper.GetInteger(GetValue(nameof(IntegrationSettingsID)), 0);
            set => SetValue(nameof(IntegrationSettingsID), value);
        }


        /// <summary>
        /// Shopify url.
        /// </summary>
        [DatabaseField]
        public virtual string ShopifyUrl
        {
            get => ValidationHelper.GetString(GetValue(nameof(ShopifyUrl)), String.Empty);
            set => SetValue(nameof(ShopifyUrl), value);
        }


        /// <summary>
        /// Admin api key.
        /// </summary>
        [DatabaseField]
        public virtual string AdminApiKey
        {
            get => ValidationHelper.GetString(GetValue(nameof(AdminApiKey)), String.Empty);
            set => SetValue(nameof(AdminApiKey), value);
        }


        /// <summary>
        /// Storefront api key.
        /// </summary>
        [DatabaseField]
        public virtual string StorefrontApiKey
        {
            get => ValidationHelper.GetString(GetValue(nameof(StorefrontApiKey)), String.Empty);
            set => SetValue(nameof(StorefrontApiKey), value);
        }


        /// <summary>
        /// Storefront api version.
        /// </summary>
        [DatabaseField]
        public virtual string StorefrontApiVersion
        {
            get => ValidationHelper.GetString(GetValue(nameof(StorefrontApiVersion)), String.Empty);
            set => SetValue(nameof(StorefrontApiVersion), value);
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
        /// Creates an empty instance of the <see cref="IntegrationSettingsInfo"/> class.
        /// </summary>
        public IntegrationSettingsInfo()
            : base(TYPEINFO)
        {
        }


        /// <summary>
        /// Creates a new instances of the <see cref="IntegrationSettingsInfo"/> class from the given <see cref="DataRow"/>.
        /// </summary>
        /// <param name="dr">DataRow with the object data.</param>
        public IntegrationSettingsInfo(DataRow dr)
            : base(TYPEINFO, dr)
        {
        }
    }
}
