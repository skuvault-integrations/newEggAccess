using NewEggAccess.Models.Orders;
using NewEggAccess.Shared;
using NewEggAccess.Throttling;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NewEggAccess.Services
{
	public interface INewEggOrdersService : IThrottler
	{
		Task<bool> TryGetOrdersAsync(Mark mark, CancellationToken cancellationToken);
		Task<IEnumerable<NewEggOrder>> GetModifiedOrdersAsync(DateTime startDateUtc, DateTime endDateUtc, string countryCode, Mark mark, CancellationToken cancellationToken);
	}
}