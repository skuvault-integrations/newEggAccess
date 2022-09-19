namespace NewEggAccess.Models.Items
{
	public class ItemInventory
	{
		public string SellerId { get; set; }
		public string ItemNumber { get; set; }
		public string SellerPartNumber { get; set; }
		public int FulFillmentOption { get; set; }
		public int AvailableQuantity { get; set; }
		public ItemInventoryAllocation[] InventoryAllocation { get; set; }
	}
	
	public class ItemInventoryAllocation
	{
		public string WarehouseLocation { get; set; }
		public int FulFillmentOption { get; set; }
		public int AvailableQuantity { get; set; }
	}
}
