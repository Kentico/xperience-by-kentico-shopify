using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;

namespace Kentico.Xperience.Shopify.ShoppingCart
{
    internal abstract class ShopifyStorefrontServiceBase
    {
        private readonly HttpClient httpClient;

        protected ShopifyStorefrontServiceBase(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient(ShopifyConstants.STOREFRONT_API_CLIENT_NAME);
        }

        protected async Task<GraphQLResponse<TResponse>> PostQueryAsync<TResponse>(string query, object? variables)
            => await SendRequestInternalAsync<TResponse>(query, variables, GraphQLRequestType.Query);

        protected async Task<GraphQLResponse<TResponse>> PostMutationAsync<TResponse>(string query, object? variables)
            => await SendRequestInternalAsync<TResponse>(query, variables, GraphQLRequestType.Mutation);

        private async Task<GraphQLResponse<TResponse>> SendRequestInternalAsync<TResponse>(string query, object? variables, GraphQLRequestType requestType)
        {
            // No Shopify store URL was set yet
            if (httpClient.BaseAddress is null)
            {
                return new GraphQLResponse<TResponse>();
            }

            var client = new GraphQLHttpClient(new GraphQLHttpClientOptions(), new NewtonsoftJsonSerializer(), httpClient);
            var request = new GraphQLRequest()
            {
                Query = query,
                Variables = variables
            };

            return requestType switch
            {
                GraphQLRequestType.Query => await client.SendQueryAsync<TResponse>(request),
                GraphQLRequestType.Mutation => await client.SendMutationAsync<TResponse>(request),
                _ => throw new NotImplementedException($"{requestType} is not implemented")
            };
        }
    }

    internal enum GraphQLRequestType
    {
        Query,
        Mutation
    }
}
