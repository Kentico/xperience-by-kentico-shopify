namespace Kentico.Xperience.Ecommerce.Common.ContentItemSynchronization;
public class ContentItemAddParams
{
    public required ContentItemSynchronizationBase ContentItem { get; set; }
    public required string LanguageName { get; set; }
    public int UserID { get; set; }
}
