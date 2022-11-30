using CuttingEdge.Conditions;

namespace NewEggAccess.Models.Items.Regular
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

            Value = value;
            Type = type;
            InventoryList = new InventoryList()
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