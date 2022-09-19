using NewEggAccess.Shared;
using System.Threading;
using System.Threading.Tasks;

namespace NewEggAccess.Services
{
	public interface INewEggCredsService
	{
		Task<bool> AreNewEggCredentialsValid(Mark mark, CancellationToken token);
	}
}