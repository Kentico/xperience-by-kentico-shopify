using ShopifySharp;

namespace DancingGoat.Models
{
    public class ThankYouPageViewModel
    {
        public string OrderName { get; set; }
        public string OrderStatusUrl { get; set; }

        public static ThankYouPageViewModel GetModel(Order order) =>
            new()
            {
                OrderName = order?.Name,
                OrderStatusUrl = order?.OrderStatusUrl
            };
    }
}
