using NewEggAccess.Configuration;

namespace NewEggAccess.Models.Commands.Business
{
	public class GetModifiedOrdersCommand : BaseNewEggCommand
	{
		public GetModifiedOrdersCommand(NewEggConfig config, NewEggCredentials credentials, string payload) : base(config, credentials, payload)
		{
			this.AddUrlParameter("version", "305");
		}

		public override string RelativeUrl => "/ordermgmt/order/orderinfo";
	}
}