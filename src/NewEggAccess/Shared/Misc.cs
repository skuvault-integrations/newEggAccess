using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace NewEggAccess.Shared
{
	public static class Misc
	{
		public static string ToJson( this object source )
		{
			try
			{
				if ( source == null )
					return "{}";
				else
				{
					var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
					settings.Converters.Add( new IsoDateTimeConverter() );
					var serialized = JsonConvert.SerializeObject( source, settings );
					return serialized;
				}
			}
			catch( Exception )
			{
				return "{}";
			}
		}

		public static List< List< T > > SplitToChunks< T >( this IEnumerable< T > source, int chunkSize )
		{
			var i = 0;
			var chunks = new List< List< T > >();
			
			while( i < source.Count() )
			{
				var temp = source.Skip( i ).Take( chunkSize ).ToList();
				chunks.Add( temp );
				i += chunkSize;
			}
			return chunks;
		}

		public static string ConvertFromUtcToPstStr( DateTime date )
		{
			var pacificDateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId( date, "Pacific Standard Time" );
			return pacificDateTime.ToString( "yyyy-MM-dd HH:mm:ss" );
		}

		public static DateTime ConvertFromPstToUtc( DateTime date )
		{
			var pstTimeZone = TimeZoneInfo.FindSystemTimeZoneById( "Pacific Standard Time" );
			return date.Add( pstTimeZone.BaseUtcOffset );
		}
	}
}