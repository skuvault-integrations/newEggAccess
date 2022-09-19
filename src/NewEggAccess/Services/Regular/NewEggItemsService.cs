using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NewEggAccess.Configuration;
using NewEggAccess.Models;
using NewEggAccess.Models.Commands.Regular;
using NewEggAccess.Models.Items;
using NewEggAccess.Models.Regular.Items;
using NewEggAccess.Shared;
using Newtonsoft.Json;

namespace NewEggAccess.Services.Regular
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
		public async Task<ItemInventory> GetSkuInventoryAsync(string sku, string warehouseLocationCode, Mark mark, CancellationToken cancellationToken)
		{
			var request = new GetItemInventoryRequest(type: SellerPartNumberRequestType, value: sku,
				warehouseLocationCode: warehouseLocationCode);
			var command = new GetItemInventoryCommand(base.Config, base.Credentials, request.ToJson());

			var response = await base.PutAsync(command, cancellationToken, mark, ignoreErrorHandler);

			if (response.Error == null && response.Result != null)
			{
				return JsonConvert.DeserializeObject<ItemInventory>(response.Result);
			}

			return null;
		}

		/// <summary>
		///	Update sku's quantity in specified warehouse location
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="warehouseLocation"></param>
		/// <param name="quantity"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public async Task<UpdateItemInventoryResponse> UpdateSkuQuantityAsync(string sku, string warehouseLocation, int quantity, Mark mark, CancellationToken cancellationToken)
		{
			var request = new UpdateItemInventoryRequest(SellerPartNumberRequestType,
				value: sku, warehouseLocation, quantity);
			var command = new GetItemInventoryCommand(base.Config, base.Credentials, request.ToJson());

			var response = await base.PostAsync(command, cancellationToken, mark, ignoreErrorHandler);

			if (response.Error == null)
			{
				return JsonConvert.DeserializeObject<UpdateItemInventoryResponse>(response.Result);
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
				await this.UpdateSkuQuantityAsync(skuQuantity.Key, warehouseLocationCode, skuQuantity.Value, mark, cancellationToken).ConfigureAwait(false);
			}
		}

		Func<HttpStatusCode, ErrorResponse, bool> ignoreErrorHandler = (status, error) =>
		{
			// can't find item
			return error != null
				  && error.Code == "CT026"
				  && status == HttpStatusCode.BadRequest;
		};
	}
}