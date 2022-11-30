using NewEggAccess.Models.Feeds;
using NewEggAccess.Shared;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NewEggAccess.Services
{
	public interface INewEggFeedsService : IThrottler
	{
		Task<string> UpdateItemsInventoryInBulkAsync(IEnumerable<InventoryUpdateFeedItem> inventory, Mark mark, CancellationToken cancellationToken);
		Task<FeedAcknowledgment> GetFeedStatusAsync(string feedId, Mark mark, CancellationToken cancellationToken);
	}
}