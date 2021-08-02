using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NewEggAccess.BusinessAccount.Models.Commands;
using NewEggAccess.Configuration;
using NewEggAccess.Models;
using NewEggAccess.Shared;
using Newtonsoft.Json;
using BusinessAccountItemInventory = NewEggAccess.BusinessAccount.Models.Items.ItemInventory;
using GetItemInventoryRequest = NewEggAccess.BusinessAccount.Models.Items.GetItemInventoryRequest;
using ItemInventory = NewEggAccess.Models.Items.ItemInventory;
using ItemInventoryAllocation = NewEggAccess.Models.Items.ItemInventoryAllocation;
using UpdateItemInventoryResponse = NewEggAccess.Models.Items.UpdateItemInventoryResponse;
using UpdateItemInventory = NewEggAccess.Models.Items.UpdateItemInventory;
using UpdateItemInventoryRequest = NewEggAccess.BusinessAccount.Models.Items.UpdateItemInventoryRequest;
using BusinessAccountUpdateItemInventoryResponse = NewEggAccess.BusinessAccount.Models.Items.BusinessAccountUpdateItemInventoryResponse;

using NewEggAccess.Services.Items;

namespace NewEggAccess.BusinessAccount.Services.Items
{
	public class NewEggItemsService : BaseService, INewEggItemsService
	{
		const int SellerPartNumberRequestType = 1;

		public NewEggItemsService( NewEggConfig config, NewEggCredentials credentials ) : base( credentials, config )
		{
		}

		/// <summary>
		///	Get sku's inventory on specified warehouseLocation
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="warehouseLocationCode"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public async Task< ItemInventory > GetSkuInventory( string sku, string warehouseLocationCode, CancellationToken token, Mark mark = null )
		{
			if ( mark == null )
			{
				mark = Mark.CreateNew();
			}

			var request = new GetItemInventoryRequest( type: SellerPartNumberRequestType, value: sku );

			Func< HttpStatusCode, ErrorResponse, bool > ignoreErrorHandler = ( status, error ) =>
			{
				// can't find item
				return error != null 
						&& error.Code == "CT026"
						&& status == HttpStatusCode.BadRequest;
			};

			var response = await base.PostAsync( new GetItemInventoryCommand( base.Config, base.Credentials, request.ToJson() ), token, mark, ignoreErrorHandler );
			
			if ( response.Error == null && response.Result != null )
			{
				return JsonConvert.DeserializeObject< ItemInventory >( response.Result, new NewEggGetItemInventoryResponseJsonConverter() );
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
		public async Task< UpdateItemInventoryResponse > UpdateSkuQuantityAsync( string sku, string warehouseLocationCountryCode, int quantity, CancellationToken token, Mark mark = null )
		{
			if ( mark == null )
			{
				mark = Mark.CreateNew();
			}

			var request = new UpdateItemInventoryRequest( SellerPartNumberRequestType, value: sku, quantity );

			Func< HttpStatusCode, ErrorResponse, bool > ignoreErrorHandler = ( status, error ) =>
			{
				  // can't find item
				  return error != null
						&& ( error.Code == "CT015" || error.Code == "CT055" )
						&& status == HttpStatusCode.BadRequest;
			};

			var response = await base.PutAsync( new UpdateItemInventoryCommand( base.Config, base.Credentials, request.ToJson() ), token, mark, ignoreErrorHandler );

			if ( response.Error == null )
			{
				return JsonConvert.DeserializeObject< UpdateItemInventoryResponse >( response.Result, new NewEggGetUpdateInventoryResponseJsonConverter() );
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
		public async Task UpdateSkusQuantitiesAsync( Dictionary<string, int> skusQuantities, string warehouseLocationCode, CancellationToken token, Mark mark = null )
		{
			foreach ( var skuQuantity in skusQuantities )
			{
				await this.UpdateSkuQuantityAsync( skuQuantity.Key, string.Empty, skuQuantity.Value, token, mark ).ConfigureAwait( false );
			}
		}
	}

	public class NewEggGetItemInventoryResponseJsonConverter : JsonConverter
	{		
		public override void WriteJson( JsonWriter writer, object value, JsonSerializer serializer )
		{
			throw new NotImplementedException();
		}

		public override object ReadJson( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer )
		{
			var source = serializer.Deserialize< BusinessAccountItemInventory >( reader );

			return new ItemInventory
			{
				ItemNumber = source.ItemNumber,
				SellerId = source.SellerId,
				SellerPartNumber = source.SellerPartNumber,
				InventoryAllocation = new ItemInventoryAllocation[]
				{
					new ItemInventoryAllocation
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

		public override bool CanConvert( Type type )
		{
			return typeof( ItemInventory ).IsAssignableFrom( type );
		}
	}

	public class NewEggGetUpdateInventoryResponseJsonConverter : JsonConverter
	{		
		public override void WriteJson( JsonWriter writer, object value, JsonSerializer serializer )
		{
			throw new NotImplementedException();
		}

		public override object ReadJson( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer )
		{
			var source = serializer.Deserialize< BusinessAccountUpdateItemInventoryResponse >( reader );

			return new UpdateItemInventoryResponse
			{
				ItemNumber = source.ItemNumber,
				SellerId = source.SellerId,
				SellerPartNumber = source.SellerPartNumber,
				InventoryList = new UpdateItemInventory[]
				{ 
					new UpdateItemInventory
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

		public override bool CanConvert( Type type )
		{
			return typeof( UpdateItemInventoryResponse ).IsAssignableFrom( type );
		}
	}
}