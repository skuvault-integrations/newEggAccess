using NewEggAccess.Configuration;

namespace NewEggAccess.Models.Commands.Regular
{
	public class GetBatchInventoryCommand : BaseNewEggCommand
	{
		public GetBatchInventoryCommand(NewEggConfig config, NewEggCredentials credentials, string payload) : base(config, credentials, payload) { }
		public override string RelativeUrl => "/contentmgmt/item/international/inventorylist";
	}
}