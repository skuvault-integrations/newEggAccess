using CuttingEdge.Conditions;
using NewEggAccess.Models.Items;

namespace NewEggAccess.Models.Regular.Items
{
	public class UpdateItemInventoryRequest
	{
		public int Type { get; private set; }
		public string Value { get; private set; }
		public InventoryList InventoryList { get; private set; }

		public UpdateItemInventoryRequest(int type, string value, string warehouseLocation, int quantity)
		{
			Condition.Requires(value, $"sku {value}").IsNotNullOrWhiteSpace().IsNotLongerThan(ItemInventoryRequest.MaxSellerPartNumberLength);
			Condition.Requires(warehouseLocation, "warehouseLocation").IsNotNullOrWhiteSpace();

			this.Value = value;
			this.Type = type;
			this.InventoryList = new InventoryList()
			{
				Inventory = new UpdateItemInventory[] { new UpdateItemInventory() { WarehouseLocation = warehouseLocation, AvailableQuantity = quantity } }
			};
		}
	}

	public class InventoryList
	{
		public UpdateItemInventory[] Inventory { get; set; }
	}
}