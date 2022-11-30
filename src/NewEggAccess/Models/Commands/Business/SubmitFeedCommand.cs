using NewEggAccess.Configuration;

namespace NewEggAccess.Models.Commands.Business
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
		Inventory_And_Price_Data
	}
}