using NewEggAccess.Models.Feeds;
using NewEggAccess.Shared;
using NewEggAccess.Throttling;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NewEggAccess.Services
{
	public interface INewEggFeedsService
	{
		Throttler Throttler { get; }
		Task<string> UpdateItemsInventoryInBulkAsync(IEnumerable<InventoryUpdateFeedItem> inventory, Mark mark, CancellationToken cancellationToken);
		Task<FeedAcknowledgment> GetFeedStatusAsync(string feedId, Mark mark, CancellationToken cancellationToken);
	}
}