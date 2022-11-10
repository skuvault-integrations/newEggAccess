using System;

namespace NewEggAccess.Exceptions
{
	public class NewEggException : Exception
	{
		public NewEggException(string message, Exception exception) : base(message, exception) { }
		public NewEggException(string message) : base(message) { }
	}
}