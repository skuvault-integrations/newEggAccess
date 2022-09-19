using NewEggAccess.Configuration;

namespace NewEggAccess.Models.Commands.Regular
{
	public class SubmitFeedCommand : BaseNewEggCommand
	{
		public SubmitFeedCommand(NewEggConfig config, NewEggCredentials credentials, SubmitFeedRequestTypeEnum requestType, string payload) : base(config, credentials, payload)
		{
			base.AddUrlParameter("requesttype", requestType.ToString().ToUpper());
		}

		public override string RelativeUrl => "/datafeedmgmt/feeds/submitfeed";
	}

	public enum SubmitFeedRequestTypeEnum
	{
		Inventory_Data,
		Item_Data,
		Price_Data,
		Order_Ship_Notice_Data,
		MultiChannel_Order_Data,
		Volume_Discount_Data,
		Item_Warranty_Data
	}
}