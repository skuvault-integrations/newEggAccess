using NewEggAccess.Models.Items;
using NewEggAccess.Shared;
using NewEggAccess.Throttling;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NewEggAccess.Services
{
	public interface INewEggItemsService
	{
		Throttler Throttler { get; }
		Task<ItemInventory> GetSkuInventoryAsync(string sku, string warehouseLocationCode, Mark mark, CancellationToken cancellationToken);
		Task<UpdateItemInventoryResponse> UpdateSkuQuantityAsync(string sku, string warehouseLocationCountryCode, int quantity, Mark mark, CancellationToken token);
		Task UpdateSkusQuantitiesAsync(Dictionary<string, int> skusQuantities, string warehouseLocationCode, Mark mark, CancellationToken token);
	}
}