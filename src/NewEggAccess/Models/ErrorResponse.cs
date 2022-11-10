using Newtonsoft.Json;

namespace NewEggAccess.Models
{
	public class ErrorResponse
	{
		[JsonProperty("Code")]
		public string Code { get; set; }
		[JsonProperty("Message")]
		public string Message { get; set; }
	}
}