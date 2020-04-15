﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace NewEggAccess.Throttling
{
	public sealed class Throttler : IDisposable
	{
		public int MaxQuota
		{
			get { return _maxQuota;  }
		}
		public int RemainingQuota
		{
			get { return _remainingQuota; }
		}

		/// <summary>
		///	API requests limits per defined time window
		/// </summary>
		public NewEggRateLimit RateLimit { get; private set; }

		private readonly int _maxQuota;
		private readonly int _quotaRestoreTimeInSeconds;
		private volatile int _remainingQuota;
		private readonly Timer _timer;
		private bool _timerStarted = false;
		private object _lock = new object();

		/// <summary>
		/// Throttler constructor. See code section for details
		/// </summary>
		/// <code>
		/// // Maximum request quota: 10 requests per 1 second 10 retry attempts if API limit exceeded
		/// var throttler = new Throttler( 10, 1, 10 )
		/// </code>
		/// <param name="maxQuota">Max requests per restore time interval</param>
		/// <param name="quotaRestoreTimeInSeconds">Quota restore time in seconds</param>
		public Throttler( int maxQuota, int quotaRestoreTimeInSeconds )
		{
			this._maxQuota = this._remainingQuota = maxQuota;
			this._quotaRestoreTimeInSeconds = quotaRestoreTimeInSeconds;
			this.RateLimit = NewEggRateLimit.Unknown;

			_timer = new Timer( RestoreQuota, null, Timeout.Infinite, _quotaRestoreTimeInSeconds * 1000 );
		}
		
		public async Task< TResult > ExecuteAsync< TResult >( Func< Task< TResult > > funcToThrottle )
		{
			lock ( _lock )
			{
				if ( !_timerStarted )
				{
					_timer.Change( _quotaRestoreTimeInSeconds * 1000, _quotaRestoreTimeInSeconds * 1000 );
					_timerStarted = true;
				}
			}

			while( true )
			{
				return await this.TryExecuteAsync( funcToThrottle ).ConfigureAwait( false );
			}
		}

		private async Task< TResult > TryExecuteAsync< TResult >( Func< Task< TResult > > funcToThrottle )
		{
			await this.WaitIfNeededAsync().ConfigureAwait( false );

			var result = await funcToThrottle().ConfigureAwait( false );

			return result;
		}

		private async Task WaitIfNeededAsync()
		{
			while ( true )
			{
				lock (_lock)
				{
					if (_remainingQuota > 0)
					{
						_remainingQuota--;
#if DEBUG
						Trace.WriteLine($"[{ DateTime.Now }] We have quota remains { _remainingQuota }. Continue work" );
#endif
						return;
					}
				}

#if DEBUG
				Trace.WriteLine($"[{ DateTime.Now }] Quota remain { _remainingQuota }. Waiting { _quotaRestoreTimeInSeconds } seconds to continue" );
#endif

				await Task.Delay( _quotaRestoreTimeInSeconds * 1000 ).ConfigureAwait( false );
			}
		}

		/// <summary>
		///	Releases quota that we have for each period of time
		/// </summary>
		/// <param name="state"></param>
		private void RestoreQuota( object state = null )
		{
			this._remainingQuota = this._maxQuota;

			#if DEBUG
				Trace.WriteLine($"[{ DateTime.Now }] Restored { _maxQuota } quota" );
			#endif
		}

		#region IDisposable Support
		private bool disposedValue = false;

		void Dispose( bool disposing )
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					_timer.Dispose();
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose( true );
		}
		#endregion
	}
}