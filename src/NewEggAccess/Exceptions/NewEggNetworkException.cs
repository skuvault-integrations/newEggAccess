using System;

namespace NewEggAccess.Exceptions
{
	public class NewEggNetworkException : NewEggException
	{
		public NewEggNetworkException(string message, Exception exception) : base(message, exception) { }
		public NewEggNetworkException(string message) : base(message) { }
	}

	public class NewEggUnauthorizedException : NewEggException
	{
		public NewEggUnauthorizedException(string message) : base(message) { }
	}

	public class NewEggRateLimitsExceeded : NewEggNetworkException
	{
		public NewEggRateLimitsExceeded(string message) : base(message) { }
	}
}