namespace DancingGoat.Models.WebPage.Shopify.ShoppingCartPage
{
    public class ShoppingCartTotalsViewModel
    {
        public string GrandTotal { get; set; }
        public string CouponInsertionMessage { get; set; }
        public IEnumerable<string> AppliedCoupons { get; set; }
        public string ShopifyCheckoutUrl { get; set; }
        public string[] ErrorMessages { get; set; }
    }
}
