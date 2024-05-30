//--------------------------------------------------------------------------------------------------
// <auto-generated>
//
//     This code was generated by code generator tool.
//
//     To customize the code use your own partial class. For more info about how to use and customize
//     the generated code see the documentation at https://docs.xperience.io/.
//
// </auto-generated>
//--------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using CMS.ContentEngine;
using CMS.Websites;
using Shopify;

namespace DancingGoat.Models
{
	/// <summary>
	/// Represents a page of type <see cref="StorePage"/>.
	/// </summary>
	[RegisterContentTypeMapping(CONTENT_TYPE_NAME)]
	public partial class StorePage : IWebPageFieldsSource
	{
		/// <summary>
		/// Code name of the content type.
		/// </summary>
		public const string CONTENT_TYPE_NAME = "Shopify.StorePage";


		/// <summary>
		/// Represents system properties for a web page item.
		/// </summary>
		[SystemField]
		public WebPageFields SystemFields { get; set; }


		/// <summary>
		/// StoreName.
		/// </summary>
		public string StoreName { get; set; }


		/// <summary>
		/// Bestsellers.
		/// </summary>
		public IEnumerable<WebPageRelatedItem> Bestsellers { get; set; }


		/// <summary>
		/// HotTips.
		/// </summary>
		public IEnumerable<WebPageRelatedItem> HotTips { get; set; }
	}
}