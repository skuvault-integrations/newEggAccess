using NewEggAccess.Configuration;

namespace NewEggAccess.Models.Commands
{
	public class GetFeedStatusCommand : BaseNewEggCommand
	{
		public GetFeedStatusCommand(NewEggConfig config, NewEggCredentials credentials, string payload) : base(config, credentials, payload) { }
		public override string RelativeUrl => "/datafeedmgmt/feeds/status";
	}
}