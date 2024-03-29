﻿using FluentAssertions;
using NewEggAccess.Models.Feeds;
using NewEggAccess.Services;
using NewEggAccess.Services.Regular;
using NewEggAccess.Shared;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NewEggTests.Regular
{
	[TestFixture]
	public class FeedsServiceTests : BaseRegularTest
	{
		private INewEggFeedsService _feedsService;

		[SetUp]
		public void Init()
		{
			this._feedsService = new NewEggFeedsService(base.Config, base.Credentials);
		}

		[Test]
		[Explicit]
		public async Task UpdateItemQuantitiesInBulkAsync()
		{
			var rand = new Random();
			var inventory = new List<InventoryUpdateFeedItem>
			{
				new InventoryUpdateFeedItem( TestSku1, rand.Next( 1, 100 ), WarehouseLocationCountryCode ),
				new InventoryUpdateFeedItem( TestSku2, rand.Next( 1, 100 ), WarehouseLocationCountryCode )
			};

			var feedId = await this._feedsService.UpdateItemsInventoryInBulkAsync(inventory, Mark.CreateNew(), CancellationToken.None);

			feedId.Should().NotBeNullOrWhiteSpace();
		}

		[Test]
		[Explicit]
		public async Task UpdateItemQuantitiesThereSomeItemsAreNotExist()
		{
			var rand = new Random();
			var inventory = new List<InventoryUpdateFeedItem>
			{
				new InventoryUpdateFeedItem( TestSku1, rand.Next( 1, 100 ), WarehouseLocationCountryCode ),
				new InventoryUpdateFeedItem( TestSku2, rand.Next( 1, 100 ), WarehouseLocationCountryCode ),
				new InventoryUpdateFeedItem( new Guid().ToString(), rand.Next( 1, 100 ), WarehouseLocationCountryCode )
			};

			var feedId = await this._feedsService.UpdateItemsInventoryInBulkAsync(inventory, Mark.CreateNew(), CancellationToken.None);

			feedId.Should().NotBeNullOrWhiteSpace();
		}

		[Test]
		[Explicit]
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