using System.Reflection;
using System.Text;
using CMS.ContentEngine;
using CMS.Helpers;

namespace Kentico.Xperience.Ecommerce.Common.ContentItemSynchronization;
public abstract class ContentItemSynchronizationBase
{
    private const string CODE_CHARS = "abcdefghijklmnopqrstuvwxyz0123456789";
    private const int CODE_LENGTH = 8;
    private const int NAME_LENGTH = 100;

    protected abstract string DisplayNameInternal { get; }

    public abstract string ContentTypeName { get; }

    /// <summary>
    /// Content item display name with max length of <see cref="NAME_LENGTH"/>
    /// </summary>
    public string DisplayName =>
        DisplayNameInternal.Length > NAME_LENGTH ? (DisplayNameInternal?.Substring(0, NAME_LENGTH) ?? string.Empty) : DisplayNameInternal;

    /// <summary>
    /// Generate dictionary where keys are property names and values are property values.
    /// <see cref="ContentItemSynchronizationBase"/> properties are excluded.
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, object?> ToDict()
    {
        var baseProperties = typeof(ContentItemSynchronizationBase)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name);

        var type = GetType();

        // Get only properties that are not from IContentItemBase
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
            .Where(p => !baseProperties.Contains(p.Name));

        Dictionary<string, object?> result = [];

        foreach (var property in properties)
        {
            string propertyName = property.Name;
            object? propertyValue = property.GetValue(this);

            result.Add(propertyName, propertyValue);
        }

        return result;
    }

    /// <summary>
    /// Get content item code name or generate if does not exist.
    /// Generated code name consists of <see cref="DisplayName"/> that is transformed to code name using <see cref="ValidationHelper.GetCodeName"/> 
    /// and random 8 characters long alphanumeric string. Maximum length will be <see cref="NAME_LENGTH"/>
    /// </summary>
    /// <returns>
    /// Generated code name
    /// </returns>
    public string GenerateCodeName()
    {
        var random = Random.Shared;

        string? codeName = ValidationHelper.GetCodeName(DisplayName);

        if (codeName.Length + CODE_LENGTH >= NAME_LENGTH)
        {
            codeName = codeName[..(NAME_LENGTH - CODE_LENGTH - 1)];
        }

        var sb = new StringBuilder(codeName);
        sb.Append('-');
        for (int i = 0; i < CODE_LENGTH; i++)
        {
            sb.Append(CODE_CHARS[random.Next(CODE_CHARS.Length)]);
        }

        return sb.ToString();
    }

    protected bool ReferenceModified(IEnumerable<IContentItemBase> contentItems, IEnumerable<ContentItemReference> contentItemReferences)
    {
        if (contentItems.Count() != contentItemReferences.Count())
        {
            return true;
        }

        for (int i = 0; i < contentItemReferences.Count(); i++)
        {
            var referenceObject = contentItemReferences.ElementAtOrDefault(i);
            var contentItem = contentItems.ElementAtOrDefault(i);

            if (referenceObject?.Identifier != contentItem?.SystemFields?.ContentItemGUID)
            {
                return true;
            }
        }

        return false;
    }
}
