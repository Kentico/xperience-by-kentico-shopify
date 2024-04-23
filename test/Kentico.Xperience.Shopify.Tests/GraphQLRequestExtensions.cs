using GraphQL;

namespace Kentico.Xperience.Shopify.Tests
{
    internal static class GraphQLRequestExtensions
    {
        public static object? GetProperty(this GraphQLRequest request, string propertyName)
            => request.Variables?.GetType()?.GetProperty(propertyName)?.GetValue(request.Variables);

        public static TResult? GetProperty<TResult>(this GraphQLRequest request, string propertyName)
        {
            ArgumentNullException.ThrowIfNull(request);
            object? property = request.GetProperty(propertyName);
            if (property == null)
            {
                return default;
            }

            return (TResult)Convert.ChangeType(property, typeof(TResult));
        }
    }
}
