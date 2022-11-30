using NewEggAccess.Configuration;

namespace NewEggAccess.Models.Commands.Business
{
	public class GetItemInventoryCommand : BaseNewEggCommand
	{
		public GetItemInventoryCommand(NewEggConfig config, NewEggCredentials credentials, string payload) : base(config, credentials, payload) { }
		public override string RelativeUrl => "/contentmgmt/item/inventory";
	}
}