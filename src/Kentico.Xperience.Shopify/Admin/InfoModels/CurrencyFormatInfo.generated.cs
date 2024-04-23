using System;
using System.Data;
using System.Runtime.Serialization;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using Kentico.Xperience.Shopify.Admin;

[assembly: RegisterObjectType(typeof(CurrencyFormatInfo), CurrencyFormatInfo.OBJECT_TYPE)]

namespace Kentico.Xperience.Shopify.Admin
{
    /// <summary>
    /// Data container class for <see cref="CurrencyFormatInfo"/>.
    /// </summary>
    [Serializable]
    public partial class CurrencyFormatInfo : AbstractInfo<CurrencyFormatInfo, IInfoProvider<CurrencyFormatInfo>>, IInfoWithId, IInfoWithName
    {
        /// <summary>
        /// Object type.
        /// </summary>
        public const string OBJECT_TYPE = "shopify.currencyformat";


        /// <summary>
        /// Type information.
        /// </summary>
#warning "You will need to configure the type info."
        public static readonly ObjectTypeInfo TYPEINFO = new ObjectTypeInfo(typeof(IInfoProvider<CurrencyFormatInfo>), OBJECT_TYPE, "Shopify.CurrencyFormat", "CurrencyFormatID", null, null, "CurrencyCode", "CurrencyCode", null, null, null)
        {
            TouchCacheDependencies = true,
        };


        /// <summary>
        /// Currency format ID.
        /// </summary>
        [DatabaseField]
        public virtual int CurrencyFormatID
        {
            get => ValidationHelper.GetInteger(GetValue(nameof(CurrencyFormatID)), 0);
            set => SetValue(nameof(CurrencyFormatID), value);
        }


        /// <summary>
        /// Currency code.
        /// </summary>
        [DatabaseField]
        public virtual string CurrencyCode
        {
            get => ValidationHelper.GetString(GetValue(nameof(CurrencyCode)), String.Empty);
            set => SetValue(nameof(CurrencyCode), value);
        }


        /// <summary>
        /// Currency price format.
        /// </summary>
        [DatabaseField]
        public virtual string CurrencyPriceFormat
        {
            get => ValidationHelper.GetString(GetValue(nameof(CurrencyPriceFormat)), String.Empty);
            set => SetValue(nameof(CurrencyPriceFormat), value);
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
        /// Constructor for de-serialization.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        protected CurrencyFormatInfo(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }


        /// <summary>
        /// Creates an empty instance of the <see cref="CurrencyFormatInfo"/> class.
        /// </summary>
        public CurrencyFormatInfo()
            : base(TYPEINFO)
        {
        }


        /// <summary>
        /// Creates a new instances of the <see cref="CurrencyFormatInfo"/> class from the given <see cref="DataRow"/>.
        /// </summary>
        /// <param name="dr">DataRow with the object data.</param>
        public CurrencyFormatInfo(DataRow dr)
            : base(TYPEINFO, dr)
        {
        }
    }
}