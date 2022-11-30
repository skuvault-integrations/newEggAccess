using CuttingEdge.Conditions;
using NewEggAccess.Configuration;
using NewEggAccess.Services;
using System;

namespace NewEggAccess
{
	public class NewEggFactory : INewEggFactory
	{
		private readonly NewEggConfig _config;
		private NewEggCredentials _credentials;

		public NewEggFactory(NewEggConfig config, string sellerId, string developerApiKey, string secretKey)
		{
			if (config.Platform == NewEggPlatform.NewEggCA)
			{
				throw new Exception("NewEgg CA is not supported");
			}

			Condition.Requires(developerApiKey, "developerApiKey").IsNotNullOrWhiteSpace();
			Condition.Requires(sellerId, "sellerId").IsNotNullOrWhiteSpace();
			Condition.Requires(secretKey, "secretKey").IsNotNullOrWhiteSpace();

			_credentials = new NewEggCredentials(sellerId, developerApiKey, secretKey);
			_config = config;
		}

		public INewEggFeedsService CreateFeedsService()
		{
			if (_config.Platform == NewEggPlatform.NewEgg)
			{
				return new Services.Regular.NewEggFeedsService(_config, _credentials);
			}

			return new Services.Business.NewEggFeedsService(_config, _credentials);
		}

		public INewEggItemsService CreateItemsService()
		{
			if (_config.Platform == NewEggPlatform.NewEgg)
			{
				return new Services.Regular.NewEggItemsService(_config, _credentials);
			}

			return new Services.Business.NewEggItemsService(_config, _credentials);
		}

		public INewEggOrdersService CreateOrdersService()
		{
			if (_config.Platform == NewEggPlatform.NewEgg)
			{
				return new Services.Regular.NewEggOrdersService(_config, _credentials);
			}

			return new Services.Business.NewEggOrdersService(_config, _credentials);
		}
	}
}