using FluentAssertions;
using NewEggAccess.Models.Feeds;
using NewEggAccess.Services;
using NewEggAccess.Services.Business;
using NewEggAccess.Shared;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NewEggTests.Business
{
	[TestFixture]
	public class FeedsTests : BaseBusinessTest
	{
		private INewEggFeedsService _feedsService;

		[SetUp]
		public void Init()
		{
			this._feedsService = new NewEggFeedsService(base.Config, base.Credentials);
		}

		[Test]
		public async Task UpdateItemQuantitiesInBulkAsync()
		{
			var rand = new Random();
			var inventory = new List<InventoryUpdateFeedItem>
			{
				new InventoryUpdateFeedItem( TestSku1, rand.Next( 1, 100 ) ),
				new InventoryUpdateFeedItem( TestSku2, rand.Next( 1, 100 ) )
			};

			var feedId = await this._feedsService.UpdateItemsInventoryInBulkAsync(inventory, Mark.CreateNew(), CancellationToken.None);

			feedId.Should().NotBeNullOrWhiteSpace();
		}

		[Test]
		public async Task UpdateItemQuantitiesThereSomeItemsAreNotExist()
		{
			var testSkuNotExist = "NotExistedSku";
			var rand = new Random();
			var inventory = new List<InventoryUpdateFeedItem>
			{
				new InventoryUpdateFeedItem( TestSku1, rand.Next( 1, 100 ) ),
				new InventoryUpdateFeedItem( TestSku2, rand.Next( 1, 100 ) ),
				new InventoryUpdateFeedItem( testSkuNotExist, 1 ),
				new InventoryUpdateFeedItem( testSkuNotExist, 1 )
			};

			var feedId = await this._feedsService.UpdateItemsInventoryInBulkAsync(inventory, Mark.CreateNew(), CancellationToken.None);

			feedId.Should().NotBeNullOrWhiteSpace();
		}

		[Test]
		public async Task GetFeedStatusAsync()
		{
			var feedId = await GetFeedIdAsync();
			var feedStatus = await this._feedsService.GetFeedStatusAsync(feedId, Mark.CreateNew(), CancellationToken.None);

			feedStatus.Should().NotBeNull();
		}

		private async Task<string> GetFeedIdAsync()
		{
			var inventory = new List<InventoryUpdateFeedItem>
			{
				new InventoryUpdateFeedItem( TestSku1, 100 )
			};

			return await this._feedsService.UpdateItemsInventoryInBulkAsync(inventory, Mark.CreateNew(), CancellationToken.None);
		}
	}
}