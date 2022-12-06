using System;
using CuttingEdge.Conditions;
using NewEggAccess.Configuration;
using NewEggAccess.Services;

namespace NewEggAccess
{
	public class NewEggFactory : INewEggFactory
	{
		private readonly string _developerApiKey;

		public NewEggFactory(string developerApiKey)
		{
			Condition.Requires(developerApiKey, "developerApiKey").IsNotNullOrWhiteSpace();
			_developerApiKey = developerApiKey;
		}

		public INewEggFeedsService CreateFeedsService(NewEggConfig config, string sellerId, string secretKey)
		{
			var credentials = new NewEggCredentials(sellerId, _developerApiKey, secretKey);
			if (config.Platform == NewEggPlatform.NewEgg)
			{
				return new Services.Regular.NewEggFeedsService(config, credentials);
			}

			return new Services.Business.NewEggFeedsService(config, credentials);
		}

		public INewEggItemsService CreateItemsService(NewEggConfig config, string sellerId, string secretKey)
		{
			var credentials = new NewEggCredentials(sellerId, _developerApiKey, secretKey);
			if (config.Platform == NewEggPlatform.NewEgg)
			{
				return new Services.Regular.NewEggItemsService(config, credentials);
			}

			return new Services.Business.NewEggItemsService(config, credentials);
		}

		public INewEggOrdersService CreateOrdersService(NewEggConfig config, string sellerId, string secretKey)
		{
			var credentials = new NewEggCredentials(sellerId, _developerApiKey, secretKey);
			if (config.Platform == NewEggPlatform.NewEgg)
			{
				return new Services.Regular.NewEggOrdersService(config, credentials);
			}

			return new Services.Business.NewEggOrdersService(config, credentials);
		}

		private static NewEggCredentials GetEggCredentials(NewEggConfig config, string sellerId, string secretKey, string developerApiKey)
		{
			if (config.Platform == NewEggPlatform.NewEggCA)
			{
				throw new Exception("NewEgg CA is not supported");
			}

			var credentials = new NewEggCredentials(sellerId, developerApiKey, secretKey);
			return credentials;
		}
	}
}