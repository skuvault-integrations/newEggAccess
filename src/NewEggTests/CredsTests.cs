using FluentAssertions;
using NewEggAccess.Services.Creds;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace NewEggTests
{
	[ TestFixture ]
	public class CredsTests : BaseTest
	{
		private NewEggCredsService _credsService;

		[ SetUp ]
		public void Init()
		{
			this._credsService = new NewEggCredsService( base.Config, base.Credentials );
		}

		[ Test ]
		public async Task AreNewEggCredentialsValid()
		{
			var result = await this._credsService.AreNewEggCredentialsValid( CancellationToken.None );

			result.Should().BeTrue();
		}		
	}
}
