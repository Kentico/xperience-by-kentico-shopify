using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;

using Microsoft.AspNetCore.Http;

namespace Kentico.Xperience.Shopify.ShoppingCart
{
    internal class GraphQLHttpClientFactory : IGraphQLHttpClientFactory
    {
        private readonly HttpClient httpClient;

        public GraphQLHttpClientFactory(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            httpClient = httpClientFactory.CreateClient(ShopifyConstants.STOREFRONT_API_CLIENT_NAME);
            string? buyerIP = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            if (!string.IsNullOrEmpty(buyerIP))
            {
                httpClient.DefaultRequestHeaders.Add(ShopifyConstants.STOREFRONT_API_BUYER_IP_NAME, buyerIP);
            }
        }

        /// <inheritdoc/>
        public IGraphQLClient CreateGraphQLHttpClient() => new GraphQLHttpClient(new GraphQLHttpClientOptions(), new NewtonsoftJsonSerializer(), httpClient);
    }
}
