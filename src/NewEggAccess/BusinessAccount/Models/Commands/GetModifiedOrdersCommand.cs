using CuttingEdge.Conditions;
using NewEggAccess.Configuration;

namespace NewEggAccess.BusinessAccount.Models.Commands
{
	public class GetModifiedOrdersCommand : NewEggCommand
	{
		public GetModifiedOrdersCommand( NewEggConfig config, NewEggCredentials credentials, string payload ) : base( config, credentials, GetModifiedOrdersServiceUrl )
		{
			Condition.Requires( payload, "payload" ).IsNotNullOrWhiteSpace();

			this.Payload = payload;
			this.AddUrlParameter( "version", ApiVersion );
		}
	}
}