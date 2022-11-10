using NewEggAccess.Configuration;

namespace NewEggAccess.Models.Commands.Business
{
	public class GetModifiedOrdersCommand : BaseNewEggCommand
	{
		public const string ApiVersion = "305";
		public GetModifiedOrdersCommand(NewEggConfig config, NewEggCredentials credentials, string payload) : base(config, credentials, payload)
		{
			this.AddUrlParameter("version", ApiVersion);
		}

		public override string RelativeUrl => "/ordermgmt/order/orderinfo";
	}
}