﻿using CuttingEdge.Conditions;
using NewEggAccess.Configuration;
using NewEggAccess.Models;
using NewEggAccess.Models.Commands;
using NewEggAccess.Models.Commands.Regular;
using NewEggAccess.Models.Feeds;
using NewEggAccess.Shared;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NewEggAccess.Services.Regular
{
	public class NewEggFeedsService : NewEggFeedsBaseService, INewEggFeedsService
	{
		public NewEggFeedsService(NewEggConfig config, NewEggCredentials credentials) : base(config, credentials)
		{
		}

		/// <summary>
		///	Submit feed with inventory data (batch update items inventory)
		/// </summary>
		/// <param name="inventory"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns>Feed id</returns>
		public async Task<string> UpdateItemsInventoryInBulkAsync(IEnumerable<InventoryUpdateFeedItem> inventory, Mark mark, CancellationToken cancellationToken)
		{
			Condition.Requires(inventory, "inventory").IsNotEmpty();
			var request = new NewEggEnvelopeWrapper<UpdateInventoryFeedRequestBody>() {
				NeweggEnvelope = new NewEggEnvelope<UpdateInventoryFeedRequestBody>("Inventory", new UpdateInventoryFeedRequestBody() { Inventory = new InventoryUpdateFeed(inventory) })
			};

			var command = new SubmitFeedCommand(base.Config, base.Credentials, SubmitFeedRequestTypeEnum.Inventory_Data, request.ToJson());
			return await base.UpdateItemsInventoryInBulkAsync(command, mark, cancellationToken).ConfigureAwait(false);
		}

		/// <summary>
		///	Get feed status
		/// </summary>
		/// <param name="feedId">Feed id</param>
		/// <param name="token">Cancellation token</param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task<FeedAcknowledgment> GetFeedStatusAsync(string feedId, Mark mark, CancellationToken cancellationToken)
		{
			Condition.Requires(feedId, "feedId").IsNotNullOrWhiteSpace();
			var request = new NewEggApiRequest<GetRequestStatusWrapper>()
			{
				OperationType = "GetFeedStatusRequest",
				RequestBody = new GetRequestStatusWrapper()
				{
					GetRequestStatus = new GetRequestStatus()
					{
						RequestIDList = new RequestIdList() { RequestID = feedId },
						MaxCount = 100,
						RequestStatus = "ALL"
					}
				}
			};

			var command = new GetFeedStatusCommand(base.Config, base.Credentials, request.ToJson());
			return await base.GetFeedStatusAsync(command, mark, cancellationToken).ConfigureAwait(false);
		}
	}
}