namespace NewEggAccess.Models.Items
{
	public class BatchInventoryResponse
	{
		public BatchInventoryResponseBody ResponseBody { get; set; }
	}

	public class BatchInventoryResponseBody
	{
		public ItemInventory[] ItemList { get; set; }
	}
}