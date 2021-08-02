using NewEggAccess.Configuration;
using NewEggAccess.Services.Creds;
using NewEggAccess.Services.Feeds;
using NewEggAccess.Services.Items;
using NewEggAccess.Services.Orders;

namespace NewEggAccess
{
	public interface INewEggFactory
	{
		INewEggItemsService CreateItemsService( NewEggConfig config, string sellerId, string secretKey );
		INewEggFeedsService CreateFeedsService( NewEggConfig config, string sellerId, string secretKey );
		INewEggOrdersService CreateOrdersService( NewEggConfig config, string sellerId, string secretKey );
		INewEggCredsService CreateCredsService( NewEggConfig config, string sellerId, string secretKey );
	}
}