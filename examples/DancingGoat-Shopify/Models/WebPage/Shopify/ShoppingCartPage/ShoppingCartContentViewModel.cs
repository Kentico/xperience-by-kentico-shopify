using Kentico.Xperience.Shopify;
using Kentico.Xperience.Shopify.ShoppingCart;

namespace DancingGoat.Models.WebPage.Shopify.ShoppingCartPage
{
    public class ShoppingCartContentViewModel
    {
        public IEnumerable<ShoppingCartItemViewModel> CartItems { get; set; }
        public string GrandTotal { get; set; }
        public IEnumerable<string> AppliedCoupons { get; set; }
        public string ShopifyCheckoutUrl { get; set; }

        public static ShoppingCartContentViewModel GetViewModel(ShoppingCartInfo cart, Dictionary<string, string> productImages)
        {
            return new ShoppingCartContentViewModel()
            {
                GrandTotal = cart?.GrandTotal.FormatPrice(cart.Currency),
                AppliedCoupons = cart?.DiscountCodes ?? [],
                ShopifyCheckoutUrl = cart.CartUrl,
                CartItems = cart?.Items.Select(x => new ShoppingCartItemViewModel()
                {
                    ItemName = x.Name,
                    Quantity = x.Quantity,
                    ImageUrl = GetDictValue(productImages, x.VariantGraphQLId),
                    VariantGraphQLId = x.VariantGraphQLId,
                    CartItemId = x.ShopifyCartItemId,
                    ItemPrice = x.Price.FormatPrice(cart.Currency),
                }) ?? []
            };
        }

        private static string GetDictValue(Dictionary<string, string> dict, string key)
        {
            if (dict.TryGetValue(key, out string value))
            {
                return value;
            }

            return string.Empty;
        }
    }
}
