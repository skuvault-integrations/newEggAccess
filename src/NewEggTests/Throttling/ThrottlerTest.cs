using System;
using System.Threading.Tasks;
using NewEggAccess.Configuration;
using NewEggAccess.Throttling;
using NSubstitute;
using NUnit.Framework;

namespace NewEggTests.Throttling
{
	[TestFixture]
	public class ThrottlerTest
	{
		private Throttler _throttler;
		private readonly IDelayer _delayerMock = Substitute.For<IDelayer>();

		[SetUp]
		public void Init()
		{
			var newEggConfig = new NewEggConfig(NewEggPlatform.NewEgg);
			_throttler = new Throttler(
				newEggConfig.ThrottlingOptions.MaxRequestsPerTimeInterval,
				newEggConfig.ThrottlingOptions.TimeIntervalInSec,
				_delayerMock);
		}

		[Test]
		public async Task ExecuteAsync_NotWaits_WhenRemainingIsDefault()
		{
			await _throttler.ExecuteAsync(() => Task.FromResult(new object()));

			var numberOfRetries = 0;
			await _delayerMock.Received(numberOfRetries).Delay(Arg.Any<TimeSpan>());
		}
		
		[Test]
		public async Task ExecuteAsync_NotWaits_WhenRemainingIsPositive()
		{
			_throttler.SetRateLimit("10", "1", "12/20/2022 1:36:43 AM");
			await _throttler.ExecuteAsync(() => Task.FromResult(new object()));

			var numberOfRetries = 0;
			await _delayerMock.Received(numberOfRetries).Delay(Arg.Any<TimeSpan>());
		}
		
		[Test]
		public async Task ExecuteAsync_WaitsOneTime_WhenRemainingIsZero()
		{
			var currentTimeInPst = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Pacific Standard Time");
			var resetTime = currentTimeInPst.AddSeconds(30);;
			_throttler.SetRateLimit("10", "0", resetTime.ToString("MM/dd/yyyy h:mm:ss tt"));
			await _throttler.ExecuteAsync(() => Task.FromResult(new object()));

			var numberOfRetries = 1;
			await _delayerMock.Received(numberOfRetries).Delay(Arg.Any<TimeSpan>());
		}
	}
}