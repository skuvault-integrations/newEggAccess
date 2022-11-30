namespace NewEggAccess.Models.Items
{
	public class UpdateItemInventory
	{
		public string WarehouseLocation { get; set; }
		public int AvailableQuantity { get; set; }
	}

	public class UpdateItemInventoryResponse
	{
		public string SellerId { get; set; }
		public string ItemNumber { get; set; }
		public string SellerPartNumber { get; set; }
		public UpdateItemInventory[] InventoryList { get; set; }
	}
}
