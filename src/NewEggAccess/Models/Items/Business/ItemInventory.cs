namespace NewEggAccess.Models.Items.Business
{
	public class ItemInventory
	{
		public string SellerId { get; set; }
		public string ItemNumber { get; set; }
		public string SellerPartNumber { get; set; }
		public int FulFillmentOption { get; set; }
		public int AvailableQuantity { get; set; }

		public Models.Items.ItemInventory ToRegularItemInventory() =>
			new Models.Items.ItemInventory
			{
				ItemNumber = this.ItemNumber,
				SellerId = this.SellerId,
				SellerPartNumber = this.SellerPartNumber,
				InventoryAllocation = new []
				{
					new Models.Items.ItemInventoryAllocation
					{
						AvailableQuantity = this.AvailableQuantity,
						FulFillmentOption = this.FulFillmentOption,
						WarehouseLocationCode = "USA"
					}
				}
			};
	}	
}
