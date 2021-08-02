using CuttingEdge.Conditions;
using NewEggAccess.BusinessAccount.Models.Commands;
using NewEggAccess.Configuration;
using NewEggAccess.Exceptions;
using NewEggAccess.Models.Feeds;
using NewEggAccess.Services.Feeds;
using NewEggAccess.Shared;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UpdateInventoryFeedRequestBody = NewEggAccess.BusinessAccount.Models.Feeds.UpdateInventoryFeedRequestBody;
using InventoryUpdateFeed = NewEggAccess.BusinessAccount.Models.Feeds.InventoryUpdateFeed;
using InventoryUpdateFeedItemBusinessAccount = NewEggAccess.BusinessAccount.Models.Feeds.InventoryUpdateFeedItem;

namespace NewEggAccess.BusinessAccount.Services.Feeds
{
	public class NewEggFeedsService : BaseService, INewEggFeedsService
	{
		public NewEggFeedsService( NewEggConfig config, NewEggCredentials credentials ) : base( credentials, config )
		{
		}

		/// <summary>
		///	Submit feed with inventory data (batch update items inventory)
		/// </summary>
		/// <param name="inventory"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns>Feed id</returns>
		public async Task< string > UpdateItemsInventoryInBulkAsync( IEnumerable< InventoryUpdateFeedItem > inventory, CancellationToken token, Mark mark = null )
		{
			Condition.Requires( inventory, "inventory" ).IsNotEmpty();

			if ( mark == null )
			{
				mark = Mark.CreateNew();
			}

			var request = PrepareUpdateInventoryFeedRequest(inventory);

			var command = new SubmitFeedCommand( base.Config, base.Credentials, SubmitFeedRequestTypeEnum.Inventory_And_Price_Data, request.ToJson() );

			var serverResponse = await base.PostAsync( command, token, mark, ( code, error ) => false ).ConfigureAwait( false );

			if ( serverResponse.Error == null )
			{
				var response = JsonConvert.DeserializeObject< NewEggApiResponse< UpdateInventoryFeedResponse > >( serverResponse.Result );

				if ( !response.IsSuccess )
				{
					throw new NewEggException( response.ToJson() );
				}

				return response.ResponseBody.ResponseList.First().RequestId;
			}

			return null;
		}

		/// <summary>
		///	Get feed status
		/// </summary>
		/// <param name="feedId">Feed id</param>
		/// <param name="token">Cancellation token</param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< FeedAcknowledgment > GetFeedStatusAsync( string feedId, CancellationToken token, Mark mark = null )
		{
			Condition.Requires( feedId, "feedId" ).IsNotNullOrWhiteSpace();

			if ( mark == null )
			{
				mark = Mark.CreateNew();
			}

			var request = new NewEggApiRequest< GetRequestStatusWrapper >{  
				OperationType = "GetFeedStatusRequest", 
				RequestBody = new GetRequestStatusWrapper{ 
					GetRequestStatus = new GetRequestStatus
					{
						RequestIDList = new RequestIdList{ RequestID = feedId }, 
						MaxCount = 100, 
						RequestStatus = "ALL" 
					}
				} 
			};

			var command = new GetFeedStatusCommand( base.Config, base.Credentials, request.ToJson() );

			var serverResponse = await base.PutAsync( command, token, mark, ( code, error ) => false ).ConfigureAwait( false );

			if ( serverResponse.Error == null )
			{
				var response = JsonConvert.DeserializeObject< NewEggApiResponse< FeedAcknowledgment > >( serverResponse.Result );

				if ( !response.IsSuccess )
				{
					throw new NewEggException( response.ToJson() );
				}

				return response.ResponseBody.ResponseList.FirstOrDefault();
			}

			return null;
		}

		private NewEggEnvelopeWrapper< UpdateInventoryFeedRequestBody > PrepareUpdateInventoryFeedRequest( IEnumerable< InventoryUpdateFeedItem > inventory )
		{			
			var inventoryItems = new List< InventoryUpdateFeedItemBusinessAccount >();

			foreach ( var inventoryItem in inventory )
			{
				inventoryItems.Add( new InventoryUpdateFeedItemBusinessAccount( inventoryItem.SellerPartNumber, inventoryItem.Inventory ) );				
			}

			return new NewEggEnvelopeWrapper< UpdateInventoryFeedRequestBody >
			{
				NeweggEnvelope = new NewEggEnvelope< UpdateInventoryFeedRequestBody >("Inventory", new UpdateInventoryFeedRequestBody() { Inventory = new InventoryUpdateFeed( inventoryItems ) })
			};
		}
	}
}