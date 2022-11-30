using System.Collections.Generic;

namespace NewEggAccess.Models
{
	public class NewEggApiResponse<T> where T : class, new()
	{
		public bool IsSuccess { get; set; }
		public string OperationType { get; set; }
		public string SellerID { get; set; }
		public NewEggApiResponseBody<T> ResponseBody { get; set; }
	}

	public class NewEggApiResponseBody<T>
	{
		public IEnumerable<T> ResponseList { get; set; }
	}
}