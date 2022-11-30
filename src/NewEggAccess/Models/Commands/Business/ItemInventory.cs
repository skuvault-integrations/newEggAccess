namespace NewEggAccess.Models.Business.Items
{
	public class ItemInventory
	{
		public string SellerId { get; set; }
		public string ItemNumber { get; set; }
		public string SellerPartNumber { get; set; }
		public int FulFillmentOption { get; set; }
		public int AvailableQuantity { get; set; }
	}	
}
