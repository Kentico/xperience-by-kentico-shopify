using Kentico.Xperience.Shopify.Config;
using Microsoft.Extensions.Options;
using ShopifySharp;
using ShopifySharp.Factories;
using ShopifySharp.Filters;

namespace Kentico.Xperience.Shopify.Services.InventoryService
{
    internal class ShopifyInventoryService : ShopifyServiceBase, IShopifyInventoryService
    {
        private readonly IInventoryItemService inventoryItemService;

        public ShopifyInventoryService(IOptionsMonitor<ShopifyConfig> options, IInventoryItemServiceFactory inventoryItemServiceFactory) : base(options)
        {
            inventoryItemService = inventoryItemServiceFactory.Create(shopifyCredentials);
        }

        public async Task<Dictionary<long, InventoryItem>> GetVariantsInventoryItems(IEnumerable<long> inventoryItemIDs)
        {
            return await TryCatch(
                async () => await GetVariantsInventoryItemsInternal(inventoryItemIDs),
                () => []);
        }

        private async Task<Dictionary<long, InventoryItem>> GetVariantsInventoryItemsInternal(IEnumerable<long> inventoryItemIDs)
        {
            var filter = new InventoryItemListFilter()
            {
                Ids = inventoryItemIDs,
                Fields = "id,tracked"
            };

            var result = await inventoryItemService.ListAsync(filter);
            return result.Items
                .Where(x => x.Id.HasValue)
                .ToDictionary(x => x.Id ?? 0, x => x);
        }
    }
}
