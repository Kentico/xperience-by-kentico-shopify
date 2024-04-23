using ShopifySharp;

namespace Kentico.Xperience.Shopify.Services.InventoryService
{
    public interface IShopifyInventoryService
    {
        /// <summary>
        /// Get inventory items by IDs
        /// </summary>
        /// <param name="inventoryItemIDs"></param>
        /// <returns>Dictionary where key is Shopify inventory item ID.</returns>
        Task<Dictionary<long, InventoryItem>> GetVariantsInventoryItems(IEnumerable<long> inventoryItemIDs);
    }
}
