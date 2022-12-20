using System;
using System.Threading;
using System.Threading.Tasks;
using NewEggAccess.Throttling;

namespace NewEggTests.Throttling
{
	/// <summary>
	/// Delayer that returns immediately. Could be used in tests to not spend time on waiting where it is not needed (e.g. if
	/// we do not call real API).
	/// </summary>
	public class ImmediateDelayer : IDelayer
	{
		public Task Delay(TimeSpan timeSpan)
		{
			return Task.CompletedTask;
		}
	}
}