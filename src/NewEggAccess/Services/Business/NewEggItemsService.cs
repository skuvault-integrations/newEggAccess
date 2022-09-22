using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NewEggAccess.Configuration;
using NewEggAccess.Models;
using NewEggAccess.Models.Commands.Business;
using NewEggAccess.Models.Business.Items;
using NewEggAccess.Shared;
using Newtonsoft.Json;

namespace NewEggAccess.Services.Business
{
	public class NewEggItemsService : BaseService, INewEggItemsService
	{
		const int SellerPartNumberRequestType = 1;

		public NewEggItemsService(NewEggConfig config, NewEggCredentials credentials) : base(credentials, config)
		{
		}

		/// <summary>
		///	Get sku's inventory on specified warehouseLocation
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="warehouseLocationCode"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public async Task<Models.Items.ItemInventory> GetSkuInventoryAsync(string sku, string warehouseLocationCode, Mark mark, CancellationToken cancellationToken)
		{
			var request = new GetItemInventoryRequest(type: SellerPartNumberRequestType, value: sku);

			Func<HttpStatusCode, ErrorResponse, bool> ignoreErrorHandler = (status, error) =>
			{
				return error != null
					&& error.Code == "CT026"
					&& status == HttpStatusCode.BadRequest;
			};

			var command = new GetItemInventoryCommand(base.Config, base.Credentials, request.ToJson());

			var response = await base.PostAsync(command, cancellationToken, mark, ignoreErrorHandler);

			if (response.Error == null && response.Result != null)
			{
				return JsonConvert.DeserializeObject<Models.Items.ItemInventory>(response.Result, new NewEggGetItemInventoryResponseJsonConverter());
			}

			return null;
		}

		/// <summary>
		///	Update sku's quantity
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="warehouseLocation"></param>
		/// <param name="quantity"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public async Task<Models.Items.UpdateItemInventoryResponse> UpdateSkuQuantityAsync(string sku, string warehouseLocationCountryCode, int quantity, Mark mark, CancellationToken cancellationToken)
		{
			var request = new UpdateItemInventoryRequest(SellerPartNumberRequestType, value: sku, quantity);

			Func<HttpStatusCode, ErrorResponse, bool> ignoreErrorHandler = (status, error) =>
			{
				return error != null
					&& (error.Code == "CT015" || error.Code == "CT055")
					&& status == HttpStatusCode.BadRequest;
			};

			var command = new UpdateItemInventoryCommand(base.Config, base.Credentials, request.ToJson());
			var response = await base.PutAsync(command, cancellationToken, mark, ignoreErrorHandler);
			if (response.Error == null)
			{
				return JsonConvert.DeserializeObject<Models.Items.UpdateItemInventoryResponse>(response.Result, new NewEggGetUpdateInventoryResponseJsonConverter());
			}

			return null;
		}

		/// <summary>
		///	Updates skus quantities
		/// </summary>
		/// <param name="skusQuantities"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task UpdateSkusQuantitiesAsync(Dictionary<string, int> skusQuantities, string warehouseLocationCode, Mark mark, CancellationToken cancellationToken)
		{
			foreach (var skuQuantity in skusQuantities)
			{
				await this.UpdateSkuQuantityAsync(skuQuantity.Key, string.Empty, skuQuantity.Value, mark, cancellationToken).ConfigureAwait(false);
			}
		}
	}

	internal class NewEggGetItemInventoryResponseJsonConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var source = serializer.Deserialize<ItemInventory>(reader);

			return new Models.Items.ItemInventory
			{
				ItemNumber = source.ItemNumber,
				SellerId = source.SellerId,
				SellerPartNumber = source.SellerPartNumber,
				InventoryAllocation = new Models.Items.ItemInventoryAllocation[]
				{
					new Models.Items.ItemInventoryAllocation
					{
						AvailableQuantity = source.AvailableQuantity,
						FulFillmentOption = source.FulFillmentOption,
						WarehouseLocationCode = "USA"
					}
				}
			};
		}

		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanConvert(Type type)
		{
			return typeof(Models.Items.ItemInventory).IsAssignableFrom(type);
		}
	}

	public class NewEggGetUpdateInventoryResponseJsonConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var source = serializer.Deserialize<UpdateItemInventoryResponse>(reader);

			return new Models.Items.UpdateItemInventoryResponse
			{
				ItemNumber = source.ItemNumber,
				SellerId = source.SellerId,
				SellerPartNumber = source.SellerPartNumber,
				InventoryList = new Models.Items.UpdateItemInventory[]
				{
					new Models.Items.UpdateItemInventory
					{
						WarehouseLocation = "USA",
						AvailableQuantity = source.AvailableQuantity
					}
				}
			};
		}

		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanConvert(Type type)
		{
			return typeof(Models.Items.UpdateItemInventoryResponse).IsAssignableFrom(type);
		}
	}
}