using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;

namespace Kentico.Xperience.Shopify.ShoppingCart
{
    public interface IGraphQLHttpClientFactory
    {
        IGraphQLClient CreateGraphQLHttpClient();
    }
}
