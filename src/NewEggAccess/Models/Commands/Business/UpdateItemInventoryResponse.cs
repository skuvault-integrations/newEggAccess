namespace NewEggAccess.Models.Business.Items
{	
	public class UpdateItemInventoryResponse
	{
		public string SellerId { get; set; }
		public string ItemNumber { get; set; }
		public string SellerPartNumber { get; set; }
		public int AvailableQuantity { get; set; }
	}
}
