using FluentAssertions;
using NewEggAccess.Models.Items;
using NewEggAccess.Services;
using NewEggAccess.Services.Regular;
using NewEggAccess.Shared;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NewEggTests.Regular
{
    [TestFixture]
	public class ItemsServiceTests : BaseRegularTest
	{
		private INewEggItemsService _itemsService;

		[SetUp]
		public void Init()
		{
			this._itemsService = new NewEggItemsService(base.Config, base.Credentials);
		}

		[Test]
		[Explicit]
		public async Task GetItemInventoryThatExists()
		{
			var itemInventory = await this._itemsService.GetSkuInventoryAsync(TestSku1, WarehouseLocationCountryCode, Mark.CreateNew(),
				CancellationToken.None);

			itemInventory.SellerPartNumber.ToLower().Should().Be(TestSku1.ToLower());
			itemInventory.InventoryAllocation.First().AvailableQuantity.Should().BeGreaterThan(0);
		}

		[Test]
		[Explicit]
		public async Task GetItemInventoryThatDoesntExist()
		{
			var sku = Guid.NewGuid().ToString();
			var itemInventory = await this._itemsService.GetSkuInventoryAsync(sku, WarehouseLocationCountryCode, Mark.CreateNew(),
				CancellationToken.None);
			itemInventory.Should().BeNull();
		}

		[Test]
		[Explicit]
		public void GetItemInventory_WhenSkuTooLong_ShouldThrow()
		{
			var longSku = new string('a', ItemInventoryRequest.MaxSellerPartNumberLength + 1);

			Assert.ThrowsAsync<ArgumentException>(async () =>
			{
				await this._itemsService.GetSkuInventoryAsync(longSku, WarehouseLocationCountryCode, Mark.CreateNew(), CancellationToken.None);
			});
		}

		[Test]
		[Explicit]
		public async Task UpdateItemInventoryThatExists()
		{
			var quantity = new Random().Next(1, 100);
			var itemInventory = await this._itemsService.UpdateSkuQuantityAsync(TestSku1, WarehouseLocationCountryCode, quantity, Mark.CreateNew(),
				CancellationToken.None);

			itemInventory.Should().NotBeNull();
			itemInventory.SellerPartNumber.ToLower().Should().Be(TestSku1.ToLower());
			itemInventory.InventoryList.First().AvailableQuantity.Should().Be(quantity);
		}

		[Test]
		[Explicit]
		public async Task UpdateItemInventoryThatDoesntExist()
		{
			var quantity = new Random().Next(1, 100);
			var sku = Guid.NewGuid().ToString();
			var itemInventory = await this._itemsService.UpdateSkuQuantityAsync(sku, WarehouseLocationCountryCode, quantity, Mark.CreateNew(),
				CancellationToken.None);

			itemInventory.Should().BeNull();
		}

		[Test]
		[Explicit]
		public async Task UpdateItemsInventory_WhenSkuAndQuantityDuplicatedAndDoesntExist_ShouldNotThrowCT055Error()
		{
			var inventory = new Dictionary<string, int>
			{
				{ "NotExistedSku", 1 }
			};

			for (var i = 0; i < 6; i++)
			{
				await this._itemsService.UpdateSkusQuantitiesAsync(inventory, WarehouseLocationCountryCode,
					Mark.CreateNew(), CancellationToken.None);
				await Task.Delay(1000);
			}
		}

		[Test]
		[Explicit]
		public async Task UpdateItemsInventory()
		{
			var rand = new Random();
			var inventory = new Dictionary<string, int>
			{
				{ TestSku1, rand.Next( 1, 100 ) },
				{ TestSku2, rand.Next( 1, 100 ) }
			};

			await this._itemsService.UpdateSkusQuantitiesAsync(inventory, WarehouseLocationCountryCode, Mark.CreateNew(), 
				CancellationToken.None);
		}

		[Test]
		[Explicit]
		public void UpdateItemInventory_WhenSkuTooLong_ShouldThrow()
		{
			var quantity = new Random().Next(1, 100);
			var longSku = new string('a', ItemInventoryRequest.MaxSellerPartNumberLength + 1); ;

			Assert.ThrowsAsync<ArgumentException>(async () =>
			{
				await this._itemsService.UpdateSkuQuantityAsync(longSku, WarehouseLocationCountryCode, quantity, Mark.CreateNew(),
				CancellationToken.None);
			});
		}

		[Test]
		[Explicit]
		public async Task GetBatchInventoryThatExists()
		{
			var response = await _itemsService.GetBatchInventoryAsync(new List<string> { TestSku1, TestSku2 },
				WarehouseLocationCountryCode, Mark.CreateNew(),
				CancellationToken.None);
			response.Count.Should().Be(2);
			response[0].SellerPartNumber.ToLower().Should().Be(TestSku1.ToLower());
			response[1].SellerPartNumber.ToLower().Should().Be(TestSku2.ToLower());
		}

		// "PIXEL 4 VZW ORANGE 64GB (A)"
		[Test]
		[Explicit]
		public async Task GetBatchInventoryThatDoesntExist()
		{
			var sku = Guid.NewGuid().ToString();
			var sku2 = Guid.NewGuid().ToString();
			var response = await this._itemsService.GetBatchInventoryAsync(new List<string> { sku, sku2 }, WarehouseLocationCountryCode, Mark.CreateNew(),
				CancellationToken.None);
			response.Count.Should().Be(0);
		}
	}
}