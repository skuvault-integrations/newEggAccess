using NewEggAccess.Services;

namespace NewEggAccess
{
	public interface INewEggFactory
	{
		INewEggFeedsService CreateFeedsService();
		INewEggItemsService CreateItemsService();
		INewEggOrdersService CreateOrdersService();
	}
}