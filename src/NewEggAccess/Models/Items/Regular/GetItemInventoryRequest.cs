using CuttingEdge.Conditions;

namespace NewEggAccess.Models.Items.Regular
{
   public class GetItemInventoryRequest
    {
        public int Type { get; private set; }
        public string Value { get; private set; }
        public WarehouseList WarehouseList { get; private set; }
        public GetItemInventoryRequest(int type, string value, string warehouseLocationCode)
        {
            Condition.Requires(value, $"sku {value}").IsNotNullOrWhiteSpace().IsNotLongerThan(ItemInventoryRequest.MaxSellerPartNumberLength);
            Condition.Requires(warehouseLocationCode, "warehouseLocationCode").IsNotNullOrWhiteSpace();

            Value = value;
            Type = type;
            WarehouseList = new WarehouseList()
            {
                WarehouseLocation = new string[] { warehouseLocationCode }
            };
        }
    }

    public class WarehouseList
    {
        public string[] WarehouseLocation { get; set; }
    }
}