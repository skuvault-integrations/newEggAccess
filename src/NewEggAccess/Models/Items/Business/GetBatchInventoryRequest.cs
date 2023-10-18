using CuttingEdge.Conditions;

namespace NewEggAccess.Models.Items.Business
{
   public class GetBatchInventoryRequest
    {
        public int Type { get; private set; }
        public string[] Values { get; private set; }

        public GetBatchInventoryRequest(int type, string[] values)
        {
            Values = values;
            Type = type;
        }
    }
}