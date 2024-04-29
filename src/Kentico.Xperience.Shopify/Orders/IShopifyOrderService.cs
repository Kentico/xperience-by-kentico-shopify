namespace Kentico.Xperience.Shopify.Orders
{
    public interface IShopifyOrderService
    {
        Task<OrderCustomerDetails?> GetOrderCustomerDetails(string cartToken);
    }
}
