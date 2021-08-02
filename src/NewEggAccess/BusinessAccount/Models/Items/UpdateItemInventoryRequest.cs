using CuttingEdge.Conditions;

namespace NewEggAccess.BusinessAccount.Models.Items
{
	public class UpdateItemInventoryRequest
	{
		public int Type { get; private set; }
		public string Value { get; private set; }
		public int Inventory { get; private set; }

		public UpdateItemInventoryRequest( int type, string value, int quantity )
		{
			Condition.Requires( value, $"sku {value}" ).IsNotNullOrWhiteSpace().IsNotLongerThan( ItemInventoryRequest.MaxSellerPartNumberLength );
			
			this.Value = value;
			this.Type = type;
			this.Inventory = quantity;
		}
	}

	public class BusinessAccountUpdateItemInventoryResponse
	{
		public string SellerId { get; set; }
		public string ItemNumber { get; set; }
		public string SellerPartNumber { get; set; }
		public int AvailableQuantity { get; set; }
	}
}