using CuttingEdge.Conditions;

namespace NewEggAccess.Configuration
{
	public class NewEggConfig
	{
		public readonly string ApiBaseUrl = "https://api.newegg.com/marketplace";

		public readonly ThrottlingOptions ThrottlingOptions;
		public readonly NetworkOptions NetworkOptions;

		public NewEggPlatform Platform { get; private set; }
		public string WarehouseLocationCountryCode { get; private set; }

		public int OrdersPageSize { get; set; }

		public NewEggConfig(NewEggPlatform platform, ThrottlingOptions throttlingOptions, NetworkOptions networkOptions)
		{
			Condition.Requires(throttlingOptions, "throttlingOptions").IsNotNull();
			Condition.Requires(networkOptions, "networkOptions").IsNotNull();

			this.Platform = platform;
			this.ThrottlingOptions = throttlingOptions;
			this.NetworkOptions = networkOptions;
			this.OrdersPageSize = 100;
		}

		public NewEggConfig(NewEggPlatform platform) : this(platform, ThrottlingOptions.NewEggDefaultOptions, NetworkOptions.NewEggDefaultOptions)
		{ }
	}

	public class ThrottlingOptions
	{
		public int MaxRequestsPerTimeInterval { get; private set; }
		public int TimeIntervalInSec { get; private set; }

		public ThrottlingOptions(int maxRequests, int timeIntervalInSec)
		{
			Condition.Requires(maxRequests, "maxRequests").IsGreaterOrEqual(1);
			Condition.Requires(timeIntervalInSec, "timeIntervalInSec").IsGreaterOrEqual(1);

			this.MaxRequestsPerTimeInterval = maxRequests;
			this.TimeIntervalInSec = timeIntervalInSec;
		}

		public static ThrottlingOptions NewEggDefaultOptions
		{
			get
			{
				return new ThrottlingOptions(10000, 86400);
			}
		}
	}

	public class NetworkOptions
	{
		public int RequestTimeoutMs { get; private set; }
		public int RetryAttempts { get; private set; }
		public int DelayBetweenFailedRequestsInSec { get; private set; }
		public int DelayFailRequestRate { get; private set; }

		public NetworkOptions(int requestTimeoutMs, int retryAttempts, int delayBetweenFailedRequestsInSec, int delayFaileRequestRate)
		{
			Condition.Requires(requestTimeoutMs, "requestTimeoutMs").IsGreaterThan(0);
			Condition.Requires(retryAttempts, "retryAttempts").IsGreaterOrEqual(0);
			Condition.Requires(delayBetweenFailedRequestsInSec, "delayBetweenFailedRequestsInSec").IsGreaterOrEqual(0);
			Condition.Requires(delayFaileRequestRate, "delayFaileRequestRate").IsGreaterOrEqual(0);

			this.RequestTimeoutMs = requestTimeoutMs;
			this.RetryAttempts = retryAttempts;
			this.DelayBetweenFailedRequestsInSec = delayBetweenFailedRequestsInSec;
			this.DelayFailRequestRate = delayFaileRequestRate;
		}

		public static NetworkOptions NewEggDefaultOptions
		{
			get
			{
				return new NetworkOptions(5 * 60 * 1000, 10, 5, 20);
			}
		}
	}

	public enum NewEggPlatform { NewEgg, NewEggBusiness, NewEggCA }
}