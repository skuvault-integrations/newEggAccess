using FluentAssertions;
using NewEggAccess.Models.Items;
using NewEggAccess.Services.Items;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NewEggTests
{
	[ TestFixture ]
	public class ItemTests : BaseTest
	{
		private INewEggItemsService _itemsService;

		[ SetUp ]
		public void Init()
		{
			this._itemsService = new NewEggItemsService( base.Config, base.Credentials );
		}

		[ Test ]
		public async Task GetItemInventoryThatExists()
		{
			var itemInventory = await this._itemsService.GetSkuInventory( TestSku1, WarehouseLocationCountryCode, CancellationToken.None );

			itemInventory.SellerPartNumber.ToLower().Should().Be( TestSku1.ToLower() );
			itemInventory.InventoryAllocation.First().AvailableQuantity.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public async Task GetItemInventoryThatDoesntExist()
		{
			var sku = Guid.NewGuid().ToString();
			var itemInventory = await this._itemsService.GetSkuInventory(sku, WarehouseLocationCountryCode, CancellationToken.None);
			itemInventory.Should().BeNull();
		}

		[ Test ]
		public void GetItemInventory_WhenSkuTooLong_ShouldThrow()
		{
			var longSku = new string( 'a', ItemInventoryRequest.MaxSellerPartNumberLength + 1 );

			Assert.ThrowsAsync< ArgumentException >( async () =>
			{
				await this._itemsService.GetSkuInventory( longSku, WarehouseLocationCountryCode, CancellationToken.None );
			} );
		}

		[ Test ]
		public async Task UpdateItemInventoryThatExists()
		{
			var quantity = new Random().Next( 1, 100 );
			var itemInventory = await this._itemsService.UpdateSkuQuantityAsync( TestSku1, WarehouseLocationCountryCode, quantity, CancellationToken.None );

			itemInventory.Should().NotBeNull();
			itemInventory.SellerPartNumber.ToLower().Should().Be( TestSku1.ToLower() );
			itemInventory.InventoryList.First().AvailableQuantity.Should().Be( quantity );
		}

		[ Test ]
		public async Task UpdateItemInventoryThatDoesntExist()
		{
			var quantity = new Random().Next( 1, 100 );
			var sku = Guid.NewGuid().ToString();
			var itemInventory = await this._itemsService.UpdateSkuQuantityAsync( sku, WarehouseLocationCountryCode, quantity, CancellationToken.None );

			itemInventory.Should().BeNull();
		}

		[Test]
		public async Task UpdateItemsInventory_WhenSkuAndQuantityDuplicatedAndDoesntExist_ShouldNotThrowCT055Error()
		{
			var inventory = new Dictionary< string, int >
			{
				{ "NotExistedSku", 1 }
			};

			for( var i = 0; i < 6; i++ )
			{
				await this._itemsService.UpdateSkusQuantitiesAsync( inventory, WarehouseLocationCountryCode, CancellationToken.None );
				await Task.Delay( 1000 );
			}
		}

		[ Test ]
		public async Task UpdateItemsInventory()
		{
			var rand = new Random();
			var inventory = new Dictionary< string, int >
			{
				{ TestSku1, rand.Next( 1, 100 ) },
				{ TestSku2, rand.Next( 1, 100 ) }
			};

			await this._itemsService.UpdateSkusQuantitiesAsync( inventory, WarehouseLocationCountryCode, CancellationToken.None );
		}

		[ Test ]
		public void UpdateItemInventory_WhenSkuTooLong_ShouldThrow()
		{
			var quantity = new Random().Next( 1, 100 );
			var longSku = new string( 'a', ItemInventoryRequest.MaxSellerPartNumberLength + 1 ); ;

			Assert.ThrowsAsync< ArgumentException >( async () =>
			{
				await this._itemsService.UpdateSkuQuantityAsync( longSku, WarehouseLocationCountryCode, quantity, CancellationToken.None );
			} );
		}
	}
}