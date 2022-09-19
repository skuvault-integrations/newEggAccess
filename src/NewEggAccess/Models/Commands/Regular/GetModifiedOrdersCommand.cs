using NewEggAccess.Configuration;

namespace NewEggAccess.Models.Commands.Regular
{
	public class GetModifiedOrdersCommand : BaseNewEggCommand
	{
		public GetModifiedOrdersCommand(NewEggConfig config, NewEggCredentials credentials, string payload) : base(config, credentials, payload)
		{
			this.AddUrlParameter("version", "307");
		}

		public override string RelativeUrl => "/ordermgmt/order/orderinfo";
	}
}