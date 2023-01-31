using System;
using System.Threading.Tasks;

namespace NewEggAccess.Shared
{
	/// <summary>
	/// Abstraction for delaying execution according to unit testing best practices:
	/// https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices#stub-static-references.
	/// Used in tests to be able to check that we delay for expected amount of time.
	/// </summary>
	public interface IDelayer
	{
		Task Delay(TimeSpan timeSpan);
	}

	internal class Delayer : IDelayer
	{
		public Task Delay(TimeSpan timeSpan)
		{
			return Task.Delay(timeSpan);
		}
	}
}