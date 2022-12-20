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
			// Act
			await _throttler.ExecuteAsync(() => Task.FromResult(new object()));

			// Assert
			var numberOfRetries = 0;
			await _delayerMock.Received(numberOfRetries).Delay(Arg.Any<TimeSpan>());
		}
		
		[Test]
		public async Task ExecuteAsync_NotWaits_WhenRemainingIsPositive()
		{
			// Arrange
			_throttler.SetRateLimit(new NewEggRateLimit("10", "1", "12/20/2022 1:36:43 AM"));
			
			// Act
			await _throttler.ExecuteAsync(() => Task.FromResult(new object()));

			// Assert
			var numberOfRetries = 0;
			await _delayerMock.Received(numberOfRetries).Delay(Arg.Any<TimeSpan>());
		}
		
		[Test]
		public async Task ExecuteAsync_WaitsOneTime_WhenRemainingIsZero()
		{
			// Arrange
			var currentTimeInPst = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Pacific Standard Time");
			var resetTime = currentTimeInPst.AddSeconds(30);;
			_throttler.SetRateLimit(new NewEggRateLimit("10", "0", resetTime.ToString("MM/dd/yyyy h:mm:ss tt")));
			
			// Act
			await _throttler.ExecuteAsync(() => Task.FromResult(new object()));

			// Assert
			var numberOfRetries = 1;
			await _delayerMock.Received(numberOfRetries).Delay(Arg.Any<TimeSpan>());
		}
	}
}