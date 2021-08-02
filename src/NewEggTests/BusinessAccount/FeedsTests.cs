using FluentAssertions;
using NewEggAccess.Models.Feeds;
using NewEggAccess.Services.Feeds;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NewEggFeedsService = NewEggAccess.BusinessAccount.Services.Feeds.NewEggFeedsService;

namespace NewEggTests.BusinessAccount
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
				new InventoryUpdateFeedItem( TestSku1, null, rand.Next( 1, 100 ) ),
				new InventoryUpdateFeedItem( TestSku2, null, rand.Next( 1, 100 ) )
			};

			var feedId = await this._feedsService.UpdateItemsInventoryInBulkAsync( inventory, CancellationToken.None );

			feedId.Should().NotBeNullOrWhiteSpace();
		}

		[ Test ]
		public async Task UpdateItemQuantitiesThereSomeItemsAreNotExist()
		{
			var testSkuNotExist = "NotExistedSku";
			var rand = new Random();
			var inventory = new List< InventoryUpdateFeedItem> 
			{
				new InventoryUpdateFeedItem( TestSku1, null, rand.Next( 1, 100 ) ),
				new InventoryUpdateFeedItem( TestSku2, null, rand.Next( 1, 100 ) ),
				new InventoryUpdateFeedItem( testSkuNotExist, null, 1 ),
				new InventoryUpdateFeedItem( testSkuNotExist, null, 1 )
			};

			var feedId = await this._feedsService.UpdateItemsInventoryInBulkAsync( inventory, CancellationToken.None );

			feedId.Should().NotBeNullOrWhiteSpace();
		}

		[ Test ]
		public async Task GetFeedStatusAsync()
		{
			var feedId = "26LY4588NXU3V";
			var feedStatus = await this._feedsService.GetFeedStatusAsync( feedId, CancellationToken.None );

			feedStatus.Should().NotBeNull();
		}
	}
}