namespace DancingGoat.Models.WebPage.Shopify.ShoppingCartPage
{
    public class ShoppingCartTotalsViewModel
    {
        public string GrandTotal { get; set; }
        public string CouponInsertionMessage { get; set; }
        public string NextStepUrl { get; set; }
        public string NextStepName { get; set; }
        public IEnumerable<string> AppliedCoupons { get; set; }
    }
}
