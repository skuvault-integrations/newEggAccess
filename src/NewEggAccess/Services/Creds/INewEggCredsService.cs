using NewEggAccess.Shared;
using NewEggAccess.Throttling;
using System.Threading;
using System.Threading.Tasks;

namespace NewEggAccess.Services.Creds
{
	public interface INewEggCredsService
	{		
		Task< bool > AreNewEggCredentialsValid( CancellationToken token, Mark mark = null );
	}
}