using NewEggAccess.Models.Items;
using NewEggAccess.Shared;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NewEggAccess.Services
{
	public interface INewEggItemsService : IThrottler
	{
		Task<ItemInventory> GetSkuInventoryAsync(string sku, string warehouseLocationCode, Mark mark, CancellationToken cancellationToken);

		Task<BatchInventoryResponse> GetBatchInventoryAsync(List<string> skus, string warehouseLocationCode, Mark mark,
			CancellationToken cancellationToken);
		Task<UpdateItemInventoryResponse> UpdateSkuQuantityAsync(string sku, string warehouseLocationCountryCode, int quantity, Mark mark, CancellationToken token);
		Task UpdateSkusQuantitiesAsync(Dictionary<string, int> skusQuantities, string warehouseLocationCode, Mark mark, CancellationToken token);
	}
}