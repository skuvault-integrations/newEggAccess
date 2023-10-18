using System.Collections.Generic;

namespace NewEggAccess.Models.Items.Business
{
	public class BatchInventoryResponse
	{
		public BatchInventoryResponseBody ResponseBody { get; set; }
	}

	public class BatchInventoryResponseBody
	{
		public List<ItemInventory> ItemList { get; set; }
	}
}