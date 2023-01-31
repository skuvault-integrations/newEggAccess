using System;
using System.Threading.Tasks;
using NewEggAccess.Configuration;
using NewEggAccess.Shared;
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
		private readonly IDateTimeProvider _dateTimeProviderMock = Substitute.For<IDateTimeProvider>();

		[SetUp]
		public void Init()
		{
			var newEggConfig = new NewEggConfig(NewEggPlatform.NewEgg);
			_throttler = new Throttler(
				newEggConfig.ThrottlingOptions.MaxRequestsPerTimeInterval,
				newEggConfig.ThrottlingOptions.TimeIntervalInSec,
				_delayerMock,
				_dateTimeProviderMock);
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
		public async Task ExecuteAsync_WaitsOneTimeAfterSpecifyTime_WhenRemainingIsZero()
		{
			// Arrange
			var utcNow = DateTime.UtcNow;
			
			// we should truncate milliseconds because NewEgg returns time as string without milliseconds in the corresponding header
			utcNow = new DateTime(utcNow.Ticks - utcNow.Ticks % TimeSpan.TicksPerSecond, utcNow.Kind);
			const int timeIntervalInSec = 30;
			var currentTimeInPst = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(utcNow, "Pacific Standard Time");
			var resetTime = currentTimeInPst.AddSeconds(timeIntervalInSec);
			_throttler.SetRateLimit(new NewEggRateLimit("10", "0", resetTime.ToString("MM/dd/yyyy h:mm:ss tt")));
			_dateTimeProviderMock.UtcNow().Returns(utcNow);

			// Act
			await _throttler.ExecuteAsync(() => Task.FromResult(new object()));

			// Assert
			var numberOfRetries = 1;
			await _delayerMock.Received(numberOfRetries).Delay(TimeSpan.FromSeconds(timeIntervalInSec));
		}
	}
}