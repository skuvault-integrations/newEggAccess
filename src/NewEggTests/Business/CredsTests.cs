using FluentAssertions;
using NewEggAccess.Services;
using NewEggAccess.Shared;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace NewEggTests.Business
{
	[TestFixture]
	public class CredsTests : BaseBusinessTest
	{
		private NewEggCredsService _credsService;

		[SetUp]
		public void Init()
		{
			this._credsService = new NewEggCredsService(base.Config, base.Credentials);
		}

		[Test]
		public async Task AreNewEggCredentialsValid()
		{
			var result = await this._credsService.AreNewEggCredentialsValid(Mark.CreateNew(), CancellationToken.None);

			result.Should().BeTrue();
		}
	}
}
