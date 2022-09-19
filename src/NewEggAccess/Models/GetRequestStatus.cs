namespace NewEggAccess.Models
{
	public class GetRequestStatusWrapper
	{
		public GetRequestStatus GetRequestStatus { get; set; }
	}

	public class GetRequestStatus
	{
		public RequestIdList RequestIDList { get; set; }
		public int MaxCount { get; set; }
		public string RequestStatus { get; set; }
	}

	public class RequestIdList
	{
		public string RequestID { get; set; }
	}
}