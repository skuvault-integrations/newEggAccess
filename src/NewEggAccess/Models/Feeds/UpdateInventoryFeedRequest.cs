using System.Collections.Generic;
using CuttingEdge.Conditions;
using NewEggAccess.Models.Items;

namespace NewEggAccess.Models.Feeds
{
    public class UpdateInventoryFeedRequestBody
	{
		public InventoryUpdateFeed Inventory { get; set; }
	}

	public class InventoryUpdateFeed
	{
		public IEnumerable<InventoryUpdateFeedItem> Item { get; private set; }

		public InventoryUpdateFeed(IEnumerable<InventoryUpdateFeedItem> items)
		{
			Condition.Requires(items, "items ").IsNotEmpty();

			this.Item = items;
		}
	}

	public class InventoryUpdateFeedItem
	{
		public string SellerPartNumber { get; private set; }
		public int Inventory { get; private set; }
		public string WarehouseLocation { get; private set; }

		public InventoryUpdateFeedItem(string sellerPartNumber, int inventory)
		{
			Condition.Requires(sellerPartNumber, "sku/sellerPartNumber").IsNotNullOrWhiteSpace().IsNotLongerThan(ItemInventoryRequest.MaxSellerPartNumberLength);
			Condition.Requires(inventory, "inventory").IsGreaterOrEqual(0);

			this.SellerPartNumber = sellerPartNumber;
			this.Inventory = inventory;
		}

		public InventoryUpdateFeedItem(string sellerPartNumber, int inventory, string warehouseLocation) : this(sellerPartNumber, inventory)
		{
			if (string.IsNullOrWhiteSpace(warehouseLocation))
				warehouseLocation = "USA";
			
			Condition.Requires(warehouseLocation, "warehouseLocation").IsNotNullOrWhiteSpace().HasLength(3);
			
			this.WarehouseLocation = warehouseLocation;
		}
	}
}