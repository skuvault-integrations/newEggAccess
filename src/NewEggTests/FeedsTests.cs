using FluentAssertions;
using NewEggAccess.Models.Feeds;
using NewEggAccess.Services.Feeds;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NewEggTests
{
	[ TestFixture ]
	public class FeedsTests : BaseTest
	{
		private INewEggFeedsService _feedsService;

		[ SetUp ]
		public void Init()
		{
			this._feedsService = new NewEggFeedsService( base.Config, base.Credentials );
		}

		[ Test ]
		public async Task UpdateItemQuantitiesInBulkAsync()
		{
			var rand = new Random();
			var inventory = new List< InventoryUpdateFeedItem> 
			{
				new InventoryUpdateFeedItem( TestSku1, WarehouseLocationCountryCode, rand.Next( 1, 100 ) ),
				new InventoryUpdateFeedItem( TestSku2, WarehouseLocationCountryCode, rand.Next( 1, 100 ) )
			};

			var feedId = await this._feedsService.UpdateItemsInventoryInBulkAsync( inventory, CancellationToken.None );

			feedId.Should().NotBeNullOrWhiteSpace();
		}

		[ Test ]
		public async Task UpdateItemQuantitiesThereSomeItemsAreNotExist()
		{
			var rand = new Random();
			var inventory = new List< InventoryUpdateFeedItem> 
			{
				new InventoryUpdateFeedItem( TestSku1, WarehouseLocationCountryCode, rand.Next( 1, 100 ) ),
				new InventoryUpdateFeedItem( TestSku2, WarehouseLocationCountryCode, rand.Next( 1, 100 ) ),
				new InventoryUpdateFeedItem( new Guid().ToString(), WarehouseLocationCountryCode, rand.Next( 1, 100 ) )
			};

			var feedId = await this._feedsService.UpdateItemsInventoryInBulkAsync( inventory, CancellationToken.None );

			feedId.Should().NotBeNullOrWhiteSpace();
		}

		[ Test ]
		public async Task GetFeedStatusAsync()
		{
			var feedId = "23FZKD2FCBZ06";
			var feedStatus = await this._feedsService.GetFeedStatusAsync( feedId, CancellationToken.None );

			feedStatus.Should().NotBeNull();
		}
	}
}