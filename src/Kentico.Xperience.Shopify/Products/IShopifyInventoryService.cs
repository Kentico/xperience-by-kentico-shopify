using ShopifySharp;

namespace Kentico.Xperience.Shopify.Products
{
    internal interface IShopifyInventoryService
    {
        /// <summary>
        /// Get inventory items by IDs
        /// </summary>
        /// <param name="inventoryItemIDs">Shopify inventory ID.</param>
        /// <returns>Dictionary where key is Shopify inventory item ID.</returns>
        Task<Dictionary<long, InventoryItem>> GetVariantsInventoryItems(IEnumerable<long> inventoryItemIDs);
    }
}
