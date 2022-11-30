using System;

namespace NewEggAccess.Models.Feeds
{
	public class FeedAcknowledgment
	{
		public DateTime RequestDate { get; set; }
		public string RequestId { get; set; }
		public FeedStatusEnum RequestStatus { get; set; }
		public string RequestType { get; set; }
		public string Memo { get; set; }
	}

	public enum FeedStatusEnum
	{
		Submitted,
		In_Progress,
		Finished,
		Cancelled
	}
}