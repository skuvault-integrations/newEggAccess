using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NewEggAccess.Configuration;
using NewEggAccess.Models;
using NewEggAccess.Models.Commands.Business;
using NewEggAccess.Shared;
using Newtonsoft.Json;
using NewEggAccess.Models.Items.Business;
using NewEggAccess.Services.Business.Converters;
using BatchInventoryResponse = NewEggAccess.Models.Items.BatchInventoryResponse;

namespace NewEggAccess.Services.Business
{
	/// <summary>
	/// Service for inventory sync 
	/// (specific for NewEgg Business Account - neweggbusiness.com)
	/// </summary>
	public class NewEggItemsService : BaseService, INewEggItemsService
	{
		const int SellerPartNumberRequestType = 1;

		public NewEggItemsService(NewEggConfig config, NewEggCredentials credentials) : base(credentials, config)
		{
		}

		/// <summary>
		/// Get sku's inventory on specified warehouseLocation
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
				return JsonConvert.DeserializeObject<Models.Items.ItemInventory>(response.Result, new GetItemInventoryResponseJsonConverter());
			}

			return null;
		}

		public async Task<List<Models.Items.ItemInventory>> GetBatchInventoryAsync(List<string> skus, string warehouseLocationCode, Mark mark, CancellationToken cancellationToken)
		{
			var itemsInventory = new List<Models.Items.ItemInventory>();
			// we should split skus by 100 items:
			// https://developer.newegg.com/newegg_marketplace_api/item_management/get-batch-inventory/
			var skusByChunks = skus.SplitToChunks(100);
			foreach (var skusInChunk in skusByChunks)
			{
				var request = new GetBatchInventoryRequest(
					SellerPartNumberRequestType,
					skusInChunk.ToArray());
				var command = new GetBatchInventoryCommand(Config, Credentials, request.ToJson());

				bool IgnoreErrorHandler(HttpStatusCode status, ErrorResponse error) => 
					error != null && error.Code == "CT026" && status == HttpStatusCode.BadRequest;

				var response = await PostAsync(command, cancellationToken, mark, IgnoreErrorHandler);

				if (response.Error == null && response.Result != null)
				{
					var batchInventoryResponse =
						JsonConvert.DeserializeObject<BatchInventoryResponse>(response.Result,
							new GetBatchInventoryResponseJsonConverter());
					itemsInventory.AddRange(batchInventoryResponse.ResponseBody.ItemList);
				}
			}

			return itemsInventory;
		}

		/// <summary>
		/// Update sku's quantity
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
				return JsonConvert.DeserializeObject<Models.Items.UpdateItemInventoryResponse>(response.Result, new GetUpdateInventoryResponseJsonConverter());
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
}