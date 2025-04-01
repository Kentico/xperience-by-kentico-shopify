using CMS.Core;

using Kentico.Xperience.Shopify.Config;

using Microsoft.Extensions.Logging;

using ShopifySharp.Credentials;
using ShopifySharp;

namespace Kentico.Xperience.Shopify.Products
{
    internal abstract class ShopifyServiceBase
    {
        protected readonly ShopifyApiCredentials shopifyCredentials;
        protected readonly ILogger<ShopifyServiceBase> logger;

        protected ShopifyServiceBase(IShopifyIntegrationSettingsService integrationSettingsService)
        {
            var settings = integrationSettingsService.GetSettings();
            string url = settings?.ShopifyUrl ?? string.Empty;
            string apiToken = settings?.AdminApiKey ?? string.Empty;
            shopifyCredentials = new ShopifyApiCredentials(url, apiToken);
            logger = Service.Resolve<ILogger<ShopifyServiceBase>>();
        }

        /// <summary>
        /// Shopify API returns HTTP 404 response when data does not exist.
        /// In order to return default value, the API call needs to be wrapped
        /// into try catch block.
        /// </summary>
        /// <typeparam name="T">Object of this type will be returned.</typeparam>
        /// <param name="func">Function to be executed inside try catch block.</param>
        /// <param name="defaultValue">Function to get default value if <see cref="ShopifyGraphErrorsException"/> is thrown.</param>
        /// <returns>Result of <paramref name="func"/> or result of <paramref name="defaultValue"/> if <see cref="ShopifyGraphErrorsException"/> is thrown.</returns>
        protected async Task<T> TryCatch<T>(Func<Task<T>> func, Func<T> defaultValue)
        {
            try
            {
                var result = await func.Invoke();
                return result;
            }
            catch (ShopifyGraphErrorsException e)
            {
                logger.LogError(e, $"Error occured while calling Shopify API in {func.Method.Name} method.");
                return defaultValue.Invoke();
            }
        }
    }
}

