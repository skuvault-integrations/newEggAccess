using CuttingEdge.Conditions;

namespace NewEggAccess.Configuration
{
	public class NewEggCredentials
	{
		public string SellerId { get; private set; }
		public string ApiKey { get; private set; }
		public string SecretKey { get; private set; }

		public NewEggCredentials(string sellerId, string apiKey, string secretKey)
		{
			Condition.Requires(sellerId, "sellerId").IsNotNullOrWhiteSpace();
			Condition.Requires(apiKey, "apiKey").IsNotNullOrWhiteSpace();
			Condition.Requires(secretKey, "secretKey").IsNotNullOrWhiteSpace();

			this.SellerId = sellerId;
			this.ApiKey = apiKey;
			this.SecretKey = secretKey;
		}
	}
}