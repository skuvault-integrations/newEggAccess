using NewEggAccess.Throttling;

namespace NewEggAccess.Services
{
	public interface IThrottler
	{
		Throttler Throttler { get; }
	}
}
