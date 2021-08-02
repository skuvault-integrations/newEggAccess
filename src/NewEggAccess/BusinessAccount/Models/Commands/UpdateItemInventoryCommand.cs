using CuttingEdge.Conditions;
using NewEggAccess.Configuration;

namespace NewEggAccess.BusinessAccount.Models.Commands
{
	public class UpdateItemInventoryCommand : NewEggCommand
	{
		public UpdateItemInventoryCommand( NewEggConfig config, NewEggCredentials credentials, string payload ) : base( config, credentials, UpdateItemInventoryServiceUrl )
		{
			Condition.Requires( payload, "payload" ).IsNotNullOrWhiteSpace();

			this.Payload = payload;
		}
	}
}