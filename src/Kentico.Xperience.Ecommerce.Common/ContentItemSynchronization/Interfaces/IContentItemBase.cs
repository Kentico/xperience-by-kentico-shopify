using CMS.ContentEngine;

namespace Kentico.Xperience.Ecommerce.Common.ContentItemSynchronization;
public interface IContentItemBase : IContentItemFieldsSource
{
    string ContentTypeName { get; }
    string DisplayName { get; }
    string ShopifyObjectID { get; }
    int ContentItemIdentifier { get; }
}
