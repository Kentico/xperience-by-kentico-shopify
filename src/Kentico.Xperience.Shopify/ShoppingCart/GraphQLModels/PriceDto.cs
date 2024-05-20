using ShopifySharp.GraphQL;

namespace Kentico.Xperience.Shopify.ShoppingCart.GraphQLModels
{
    internal class PriceDto
    {
        public decimal Amount { get; set; }
        public CurrencyCode CurrencyCode { get; set; }
    }
}
