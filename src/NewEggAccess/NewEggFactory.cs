using CuttingEdge.Conditions;
using NewEggAccess.Configuration;
using NewEggAccess.Services.Creds;
using NewEggAccess.Services.Feeds;
using NewEggAccess.Services.Items;
using NewEggAccess.Services.Orders;

namespace NewEggAccess
{
	public class NewEggFactory : INewEggFactory
	{
		private string _developerApiKey;

		public NewEggFactory( string developerApiKey )
		{
			Condition.Requires( developerApiKey, "developerApiKey" ).IsNotNullOrWhiteSpace();

			this._developerApiKey = developerApiKey;
		}

		public INewEggFeedsService CreateFeedsService( NewEggConfig config, string sellerId, string secretKey )
		{
			var credentials = new NewEggCredentials( sellerId, this._developerApiKey, secretKey );
			
			if ( config.Platform == NewEggPlatform.NewEgg )
				return new NewEggFeedsService( config, credentials );

			return new BusinessAccount.Services.Feeds.NewEggFeedsService( config, credentials );
		}

		public INewEggItemsService CreateItemsService( NewEggConfig config, string sellerId, string secretKey )
		{
			var credentials = new NewEggCredentials( sellerId, this._developerApiKey, secretKey );
			
			if ( config.Platform == NewEggPlatform.NewEgg )
				return new NewEggItemsService( config, credentials );

			return new BusinessAccount.Services.Items.NewEggItemsService( config, credentials );
		}

		public INewEggOrdersService CreateOrdersService( NewEggConfig config, string sellerId, string secretKey )
		{
			var credentials = new NewEggCredentials( sellerId, this._developerApiKey, secretKey );

			if ( config.Platform == NewEggPlatform.NewEgg )
				return new NewEggOrdersService( config, credentials );

			return new BusinessAccount.Services.Orders.NewEggOrdersService( config, credentials );
		}

		public INewEggCredsService CreateCredsService( NewEggConfig config, string sellerId, string secretKey )
		{
			var credentials = new NewEggCredentials( sellerId, this._developerApiKey, secretKey );

			if ( config.Platform == NewEggPlatform.NewEgg )
				return new NewEggCredsService( config, credentials );

			return new BusinessAccount.Services.Creds.NewEggCredsService( config, credentials );
		}
	}
}