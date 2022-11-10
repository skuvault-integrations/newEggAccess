using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NewEggAccess.Configuration;
using NewEggAccess.Exceptions;
using NewEggAccess.Models;
using NewEggAccess.Models.Commands;
using NewEggAccess.Models.Feeds;
using NewEggAccess.Shared;
using Newtonsoft.Json;

namespace NewEggAccess.Services
{
	public class NewEggBaseFeedsService : BaseService
	{
		public NewEggBaseFeedsService(NewEggConfig config, NewEggCredentials credentials) : base(credentials, config)
		{
		}

		/// <summary>
		///	Submit feed with inventory data (batch update items inventory)
		/// </summary>
		/// <param name="inventory"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns>Feed id</returns>
		public async Task<string> UpdateItemsInventoryInBulkAsync(BaseNewEggCommand command, Mark mark, CancellationToken token)
		{
			var serverResponse = await base.PostAsync(command, token, mark, (code, error) => false).ConfigureAwait(false);

			if (serverResponse.Error == null)
			{
				var response = JsonConvert.DeserializeObject<NewEggApiResponse<UpdateInventoryFeedResponse>>(serverResponse.Result);

				if (!response.IsSuccess)
				{
					throw new NewEggException(response.ToJson());
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
		public async Task<FeedAcknowledgment> GetFeedStatusAsync(BaseNewEggCommand command, Mark mark, CancellationToken token)
		{
			var serverResponse = await base.PutAsync(command, token, mark, (code, error) => false).ConfigureAwait(false);

			if (serverResponse.Error == null)
			{
				var response = JsonConvert.DeserializeObject<NewEggApiResponse<FeedAcknowledgment>>(serverResponse.Result);

				if (!response.IsSuccess)
				{
					throw new NewEggException(response.ToJson());
				}

				return response.ResponseBody.ResponseList.FirstOrDefault();
			}

			return null;
		}
	}
}