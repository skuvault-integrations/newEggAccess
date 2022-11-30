using CuttingEdge.Conditions;

namespace NewEggAccess.Models.Items.Business
{
    public class UpdateItemInventoryRequest
    {
        public int Type { get; private set; }
        public string Value { get; private set; }
        public int Inventory { get; private set; }

        public UpdateItemInventoryRequest(int type, string value, int quantity)
        {
            Condition.Requires(value, $"sku {value}").IsNotNullOrWhiteSpace().IsNotLongerThan(ItemInventoryRequest.MaxSellerPartNumberLength);

            Value = value;
            Type = type;
            Inventory = quantity;
        }
    }
}