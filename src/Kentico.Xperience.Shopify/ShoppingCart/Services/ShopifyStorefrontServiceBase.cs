using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;

namespace Kentico.Xperience.Shopify.ShoppingCart.Services
{
    internal abstract class ShopifyStorefrontServiceBase
    {
        private readonly HttpClient httpClient;

        protected ShopifyStorefrontServiceBase(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient(ShopifyConstants.STOREFRONT_API_CLIENT_NAME);
        }

        protected async Task<GraphQLResponse<TResponse>> PostQueryAsync<TResponse>(string query, object? variables)
        {
            var client = GetClient();
            var request = new GraphQLRequest()
            {
                Query = query,
                Variables = variables
            };

            return await client.SendQueryAsync<TResponse>(request);
        }

        protected async Task<GraphQLResponse<TResponse>> PostMutationAsync<TResponse>(string query, object? variables)
        {
            var client = GetClient();
            var request = new GraphQLRequest()
            {
                Query = query,
                Variables = variables
            };
            return await client.SendMutationAsync<TResponse>(request);
        }

        private GraphQLHttpClient GetClient() => new(new GraphQLHttpClientOptions(), new NewtonsoftJsonSerializer(), httpClient);
    }
}
