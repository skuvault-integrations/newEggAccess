using System;
using System.Globalization;

namespace NewEggAccess.Throttling
{
	public class NewEggRateLimit
	{
		public int Limit { get; set; }
		public int Remaining { get; set; }
		public DateTime ResetTime { get; set; }

		public NewEggRateLimit( string limitRaw, string remainingRaw, string resetTimeRaw )
		{
			this.Limit = TryParseValue( limitRaw );
			this.Remaining = TryParseValue( remainingRaw );
			this.ResetTime = TryParseResetTime( resetTimeRaw );
		}

		public NewEggRateLimit( int limit, int remaining, DateTime resetTime )
		{
			this.Limit = limit;
			this.Remaining = remaining;
			this.ResetTime = resetTime;
		}

		private int TryParseValue( string value )
		{
			int.TryParse( value, out int result );
			return result;
		}

		private DateTime TryParseResetTime( string date )
		{
			var formats = new [] { "M/d/yyyy h:mm:ss tt" };
			DateTime.TryParseExact( date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result );
			return result;
		}
	}
}