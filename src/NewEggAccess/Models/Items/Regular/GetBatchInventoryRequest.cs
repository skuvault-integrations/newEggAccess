using CuttingEdge.Conditions;

namespace NewEggAccess.Models.Items.Regular
{
   public class GetBatchInventoryRequest
    {
        public int Type { get; private set; }
        public string[] Values { get; private set; }
        public string[] WarehouseList { get; private set; }
        public GetBatchInventoryRequest(int type, string[] values, string warehouseLocationCode)
        {
            Values = values;
            Type = type;
            WarehouseList = new [] { warehouseLocationCode };
        }
    }
}