using CuttingEdge.Conditions;

namespace NewEggAccess.Models.Orders
{
	public class GetOrderInfoRequest
	{
		public string OperationType { get; private set; }
		public GetOrderInfoRequestBody RequestBody { get; private set; }

		public GetOrderInfoRequest(GetOrderInfoRequestBody body)
		{
			Condition.Requires(body, "body").IsNotNull();

			this.OperationType = "GetOrderInfoRequest";
			this.RequestBody = body;
		}
	}

	public class GetOrderInfoRequestBody
	{
		public int PageIndex { get; private set; }
		public int PageSize { get; private set; }
		public GetOrderInfoRequestCriteria RequestCriteria { get; private set; }

		public GetOrderInfoRequestBody(int pageIndex, int pageSize, GetOrderInfoRequestCriteria criteria)
		{
			Condition.Requires(pageIndex, "pageIndex").IsGreaterOrEqual(1);
			Condition.Requires(pageSize, "pageSize").IsGreaterThan(0);
			Condition.Requires(criteria, "criteria").IsNotNull();

			this.PageIndex = pageIndex;
			this.PageSize = pageSize;
			this.RequestCriteria = criteria;
		}
	}

	public class GetOrderInfoRequestCriteria
	{
		public int Type { get; set; }
		public int Status { get; set; }
		public string OrderDateFrom { get; set; }
		public string OrderDateTo { get; set; }
		public string CountryCode { get; set; }
	}
}
