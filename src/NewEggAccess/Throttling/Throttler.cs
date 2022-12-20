using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace NewEggAccess.Throttling
{
	public sealed class Throttler
	{
		/// <summary>
		///	API requests limits per defined time window
		/// </summary>
		private NewEggRateLimit RateLimit { get; set; }
		
		private readonly int _defaultQuotaRestoreTimeInSeconds;

		private readonly object _lock = new object();

		private readonly IDelayer _delayer;

		/// <summary>
		/// Throttler constructor. See code section for details
		/// </summary>
		/// <code>
		/// // Maximum request quota: 10 requests per 1 second 10 retry attempts if API limit exceeded
		/// var throttler = new Throttler( 10, 1, 10 )
		/// </code>
		/// <param name="maxQuota">Max requests per restore time interval</param>
		/// <param name="quotaRestoreTimeInSeconds">Quota restore time in seconds</param>
		/// <param name="delayer"></param>
		public Throttler( int maxQuota, int quotaRestoreTimeInSeconds, IDelayer delayer )
		{
			_delayer = delayer;
			_defaultQuotaRestoreTimeInSeconds = quotaRestoreTimeInSeconds;
			RateLimit = new NewEggRateLimit(maxQuota, maxQuota, GetCurrentTimeInPst().AddSeconds(_defaultQuotaRestoreTimeInSeconds));
		}

		public void SetRateLimit(string limit, string remaining, string resetTime)
		{
			RateLimit = new NewEggRateLimit(limit, remaining, resetTime);
		}
		
		public async Task< TResult > ExecuteAsync< TResult >( Func< Task< TResult > > funcToThrottle )
		{
			await WaitIfNeededAsync().ConfigureAwait( false );

			var result = await funcToThrottle().ConfigureAwait( false );

			return result;
		}

		private async Task WaitIfNeededAsync()
		{
			lock (_lock)
			{
				if( RateLimit.Remaining > 0 )
				{
#if DEBUG
					Trace.WriteLine($"[{ DateTime.Now }] We have quota remains { RateLimit.Remaining }. Continue work" );
#endif
					return;
				}
			}

			var quotaRestoreTime = GetQuotaRestoreTime();
#if DEBUG
			Trace.WriteLine($"[{ DateTime.Now }] Quota remain { RateLimit.Remaining }. Waiting { quotaRestoreTime.TotalSeconds } seconds to continue" );
#endif

			await _delayer.Delay( quotaRestoreTime ).ConfigureAwait( false );
		}

		private TimeSpan GetQuotaRestoreTime()
		{
			// We should use PST time because NewEgg works in this time zone: https://developer.newegg.com/newegg_marketplace_api/newegg_marketplace_api_endpoints_and_time_standard/
			var currentTimeInPst = GetCurrentTimeInPst();

			return RateLimit.ResetTime > currentTimeInPst
				? RateLimit.ResetTime - currentTimeInPst
				: TimeSpan.FromSeconds(_defaultQuotaRestoreTimeInSeconds);
		}

		private DateTime GetCurrentTimeInPst() => TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Pacific Standard Time");
	}
}