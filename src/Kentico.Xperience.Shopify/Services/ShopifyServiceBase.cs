﻿using CMS.Core;
using Kentico.Xperience.Shopify.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ShopifySharp.Credentials;
using ShopifySharp.Infrastructure;

namespace Kentico.Xperience.Shopify.Services
{
    internal abstract class ShopifyServiceBase
    {
        protected readonly ShopifyApiCredentials shopifyCredentials;
        protected readonly ILogger<ShopifyServiceBase> logger;

        protected ShopifyServiceBase(IOptionsMonitor<ShopifyConfig> options)
        {
            string url = options.CurrentValue.ShopifyUrl;
            string apiToken = options.CurrentValue.AdminApiToken;
            shopifyCredentials = new ShopifyApiCredentials(url, apiToken);
            logger = Service.Resolve<ILogger<ShopifyServiceBase>>();
        }

        /// <summary>
        /// Shopify API returns HTTP 404 response when data does not exist.
        /// In order to return default value, the API call needs to be wrapped
        /// into try catch block.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected async Task<T> TryCatch<T>(Func<Task<T>> func, Func<T> defaultValue)
        {
            try
            {
                var result = await func.Invoke();
                return result;
            }
            catch (ShopifyHttpException e)
            {
                logger.LogError(e, $"Error occured while calling Shopify API in {func.Method.Name} method.");
                return defaultValue.Invoke();
            }
        }
    }
}

