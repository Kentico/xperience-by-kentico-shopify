using CMS.Websites;

namespace DancingGoat.Models;
public record StoreCategoryListViewModel
{
    public string CategoryName { get; set; }
    public string CategoryUrl { get; set; }

    public static async ValueTask<StoreCategoryListViewModel> GetViewModel(CategoryPage category, IWebPageUrlRetriever urlRetriever)
    {
        return new StoreCategoryListViewModel
        {
            CategoryName = category.CategoryName,
            CategoryUrl = (await urlRetriever.Retrieve(category)).RelativePath
        };
    }
}

