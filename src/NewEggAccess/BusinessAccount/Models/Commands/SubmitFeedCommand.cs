using CuttingEdge.Conditions;
using NewEggAccess.Configuration;

namespace NewEggAccess.BusinessAccount.Models.Commands
{
	public class SubmitFeedCommand : NewEggCommand
	{
		public SubmitFeedCommand( NewEggConfig config, NewEggCredentials credentials, SubmitFeedRequestTypeEnum requestType, string payload ) : base( config, credentials, SubmitFeedServiceUrl )
		{
			Condition.Requires( payload, "payload" ).IsNotNullOrWhiteSpace();

			this.Payload = payload;
			base.AddUrlParameter( "requesttype", requestType.ToString().ToUpper() );
		}
	}

	public enum SubmitFeedRequestTypeEnum
	{
		Inventory_And_Price_Data
	}
}