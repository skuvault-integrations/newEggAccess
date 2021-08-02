using CuttingEdge.Conditions;

namespace NewEggAccess.BusinessAccount.Models.Items
{
	public class GetItemInventoryRequest
	{
		public int Type { get; private set; }
		public string Value { get; private set; }		

		public GetItemInventoryRequest( int type, string value )
		{
			Condition.Requires( value, $"sku {value}" ).IsNotNullOrWhiteSpace().IsNotLongerThan( ItemInventoryRequest.MaxSellerPartNumberLength );			

			this.Value = value;
			this.Type = type;			
		}
	}
}