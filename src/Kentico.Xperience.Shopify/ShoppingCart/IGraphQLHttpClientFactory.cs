using GraphQL.Client.Abstractions;

namespace Kentico.Xperience.Shopify.ShoppingCart
{
    /// <summary>
    /// Factory for creating GraphQL HTTP clients.
    /// </summary>
    public interface IGraphQLHttpClientFactory
    {
        /// <summary>
        /// Creates a new instance of a GraphQL HTTP client.
        /// </summary>
        /// <returns>A new instance of a GraphQL client.</returns>
        IGraphQLClient CreateGraphQLHttpClient();
    }
}
