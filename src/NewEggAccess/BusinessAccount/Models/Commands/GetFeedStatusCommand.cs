using CuttingEdge.Conditions;
using NewEggAccess.Configuration;

namespace NewEggAccess.BusinessAccount.Models.Commands
{
	public class GetFeedStatusCommand : NewEggCommand
	{
		public GetFeedStatusCommand( NewEggConfig config, NewEggCredentials credentials, string payload ) : base( config, credentials, GetFeedStatusServiceUrl )
		{
			Condition.Requires( payload, "payload" ).IsNotNullOrWhiteSpace();

			this.Payload = payload;
		}
	}
}