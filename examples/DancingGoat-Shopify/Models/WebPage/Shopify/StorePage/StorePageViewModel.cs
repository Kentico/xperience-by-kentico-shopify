using CMS.Websites;

namespace DancingGoat.Models;

public record StorePageViewModel : IWebPageBasedViewModel
{
    public string Title { get; set; }
    public IEnumerable<StoreCategoryListViewModel> Categories { get; set; }

    public IWebPageFieldsSource WebPage { get; init; }

    public static StorePageViewModel GetViewModel(StorePage store, IEnumerable<StoreCategoryListViewModel> categories)
    {
        if (store == null)
        {
            return null;
        }

        return new StorePageViewModel
        {
            Title = store.StoreName,
            Categories = categories
        };
    }
}
