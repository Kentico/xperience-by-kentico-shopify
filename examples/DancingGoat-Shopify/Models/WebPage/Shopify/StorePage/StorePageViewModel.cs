using CMS.Websites;

namespace DancingGoat.Models;

public record StorePageViewModel : IWebPageBasedViewModel
{
    public string Title { get; set; }
    public IEnumerable<StoreCategoryListViewModel> Categories { get; set; }
    public IEnumerable<ShopifyProductListItemViewModel> HotTips { get; set; }
    public IEnumerable<ShopifyProductListItemViewModel> BestSellers { get; set; }


    public IWebPageFieldsSource WebPage { get; init; }

    public static StorePageViewModel GetViewModel(
        StorePage store,
        IEnumerable<StoreCategoryListViewModel> categories,
        IEnumerable<ShopifyProductListItemViewModel> hotTips, IEnumerable<ShopifyProductListItemViewModel> bestSellers)
    {
        if (store == null)
        {
            return null;
        }

        return new StorePageViewModel
        {
            Title = store.StoreName,
            Categories = categories,
            HotTips = hotTips,
            BestSellers = bestSellers
        };
    }
}
