using System;

namespace NewEggAccess.Shared
{
	/// <summary>
	/// Abstraction for getting DateTime related props:
	/// https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices#stub-static-references.
	/// Used in tests to be able to substitute static DateTime dependency.
	/// </summary>
	public interface IDateTimeProvider
	{
		DateTime UtcNow();
	}

	public class DateTimeProvider : IDateTimeProvider
	{
		public DateTime UtcNow() => DateTime.UtcNow;
	}
}