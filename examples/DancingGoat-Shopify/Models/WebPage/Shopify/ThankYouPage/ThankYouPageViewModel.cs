using Kentico.Xperience.Shopify.Orders.Models;

namespace DancingGoat.Models
{
    public class ThankYouPageViewModel
    {
        public string OrderName { get; set; }
        public string OrderStatusUrl { get; set; }

        public static ThankYouPageViewModel GetModel(OrderModel order) =>
            new()
            {
                OrderName = order?.Name,
                OrderStatusUrl = order?.StatusPageUrl
            };
    }
}
