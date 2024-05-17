using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;

namespace Kentico.Xperience.Shopify.ShoppingCart
{
    internal class GraphQLHttpClientFactory : IGraphQLHttpClientFactory
    {
        private readonly HttpClient httpClient;

        public GraphQLHttpClientFactory(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient(ShopifyConstants.STOREFRONT_API_CLIENT_NAME);
        }

        /// <inheritdoc/>
        public IGraphQLClient CreateGraphQLHttpClient() => new GraphQLHttpClient(new GraphQLHttpClientOptions(), new NewtonsoftJsonSerializer(), httpClient);
    }
}
