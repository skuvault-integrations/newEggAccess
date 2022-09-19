using NewEggAccess.Services;

namespace NewEggAccess
{
	public interface INewEggFactory
	{
		INewEggFeedsService CreateFeedsService();
		INewEggCredsService CreateCredsService();
		INewEggItemsService CreateItemsService();
		INewEggOrdersService CreateOrdersService();
	}
}