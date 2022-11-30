using CuttingEdge.Conditions;

namespace NewEggAccess.Models.Items.Business
{
   public class GetItemInventoryRequest
    {
        public int Type { get; private set; }
        public string Value { get; private set; }

        public GetItemInventoryRequest(int type, string value)
        {
            Condition.Requires(value, $"sku {value}").IsNotNullOrWhiteSpace().IsNotLongerThan(ItemInventoryRequest.MaxSellerPartNumberLength);

            Value = value;
            Type = type;
        }
    }
}