using FluentAssertions;
using NewEggAccess.Throttling;
using NUnit.Framework;
using System;

namespace NewEggTests
{
	[TestFixture]
	public class RateLimitTests
	{
		[Test]
		public void ParseCorrectRateLimitsRespose()
		{
			var rateLimits = new NewEggRateLimit("1000", "995", "4/14/2020 8:53:25 PM");

			rateLimits.Limit.Should().Be(1000);
			rateLimits.Remaining.Should().Be(995);
			rateLimits.ResetTime.Should().Be(new DateTime(2020, 4, 14, 20, 53, 25));
		}

		[Test]
		public void ParseRateLimitsResponseWithDifferentDateFormat()
		{
			var rateLimits = new NewEggRateLimit("1000", "995", "10/4/2020 8:53:25 PM");

			rateLimits.Limit.Should().Be(1000);
			rateLimits.Remaining.Should().Be(995);
			rateLimits.ResetTime.Should().Be(new DateTime(2020, 4, 10, 20, 53, 25));
		}

		[Test]
		public void ParseRateLimitsWithUnknownDateTimeFormat()
		{
			var rateLimits = new NewEggRateLimit("1000", "995", "14 Apr 2020 8:53:25 PM");

			rateLimits.Limit.Should().Be(1000);
			rateLimits.Remaining.Should().Be(995);
			rateLimits.ResetTime.Should().Be(default(DateTime));
		}

		[Test]
		public void ParseRateLimitsWithIncorrectValues()
		{
			var rateLimits = new NewEggRateLimit("test", "test", "test");

			rateLimits.Limit.Should().Be(0);
			rateLimits.Remaining.Should().Be(0);
			rateLimits.ResetTime.Should().Be(default(DateTime));
		}
	}
}
