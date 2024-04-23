using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;

namespace Kentico.Xperience.Shopify.ShoppingCart
{
    internal abstract class ShopifyStorefrontServiceBase
    {
        private readonly IGraphQLHttpClientFactory clientFactory;

        protected ShopifyStorefrontServiceBase(IGraphQLHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        protected async Task<GraphQLResponse<TResponse>> PostQueryAsync<TResponse>(string query, object? variables)
            => await SendRequestInternalAsync<TResponse>(query, variables, GraphQLRequestType.Query);

        protected async Task<GraphQLResponse<TResponse>> PostMutationAsync<TResponse>(string query, object? variables)
            => await SendRequestInternalAsync<TResponse>(query, variables, GraphQLRequestType.Mutation);

        private async Task<GraphQLResponse<TResponse>> SendRequestInternalAsync<TResponse>(string query, object? variables, GraphQLRequestType requestType)
        {
            var client = clientFactory.CreateGraphQLHttpClient();
            var request = new GraphQLRequest()
            {
                Query = query,
                Variables = variables
            };

            try
            {
                return requestType switch
                {
                    GraphQLRequestType.Query => await client.SendQueryAsync<TResponse>(request),
                    GraphQLRequestType.Mutation => await client.SendMutationAsync<TResponse>(request),
                    _ => throw new NotImplementedException($"{requestType} is not implemented")
                };
            }
            catch (InvalidOperationException)
            {
                return new GraphQLResponse<TResponse>();
            }
        }
    }

    internal enum GraphQLRequestType
    {
        Query,
        Mutation
    }
}
