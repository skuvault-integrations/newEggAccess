using CuttingEdge.Conditions;
using NewEggAccess.Configuration;

namespace NewEggAccess.BusinessAccount.Models.Commands
{
	public class GetItemInventoryCommand : NewEggCommand
	{
		public GetItemInventoryCommand( NewEggConfig config, NewEggCredentials credentials, string payload ) : base( config, credentials, GetItemInventoryServiceUrl )
		{
			Condition.Requires( payload, "payload" ).IsNotNullOrWhiteSpace();

			this.Payload = payload;
		}
	}
}