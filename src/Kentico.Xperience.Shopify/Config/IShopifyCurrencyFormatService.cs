﻿namespace Kentico.Xperience.Shopify.Config
{
    /// <summary>
    /// Service for managing currency formats.
    /// </summary>
    public interface IShopifyCurrencyFormatService
    {
        /// <summary>
        /// Get all the currencies formats.
        /// </summary>
        /// <returns>Read-only dictionary where key is the currency IS0 4217 format and value is the price format.</returns>
        IReadOnlyDictionary<string, string> GetFormats();
    }
}
