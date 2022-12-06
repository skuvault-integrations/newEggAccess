using NewEggAccess.Configuration;
using NewEggAccess.Services;

namespace NewEggAccess
{
	public interface INewEggFactory
	{
		INewEggFeedsService CreateFeedsService(NewEggConfig config, string sellerId, string secretKey);
		INewEggItemsService CreateItemsService(NewEggConfig config, string sellerId, string secretKey);
		INewEggOrdersService CreateOrdersService(NewEggConfig config, string sellerId, string secretKey);
	}
}