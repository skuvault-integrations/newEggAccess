using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NewEggAccess.Configuration;
using NewEggAccess.Exceptions;
using NewEggAccess.Models.Commands.Business;
using NewEggAccess.Models.Orders;
using NewEggAccess.Shared;
using Newtonsoft.Json;

namespace NewEggAccess.Services
{
	public abstract class NewEggBaseOrdersService : BaseService, INewEggOrdersService
	{
		public NewEggBaseOrdersService(NewEggConfig config, NewEggCredentials credentials) : base(credentials, config)
		{
		}

		public async Task<IEnumerable<NewEggOrder>> GetModifiedOrdersAsync(DateTime startDateUtc, DateTime endDateUtc,
			string countryCode, Mark mark, CancellationToken cancellationToken)
		{
			var orders = new List<NewEggOrder>();

			// order status actually cannot be omitted in request body (API error)
			orders.AddRange(await this.GetModifiedOrdersByStatusAsync(startDateUtc, endDateUtc, NewEggOrderStatusEnum.Unshipped,
				mark, cancellationToken).ConfigureAwait(false));
			orders.AddRange(await this.GetModifiedOrdersByStatusAsync(startDateUtc, endDateUtc, NewEggOrderStatusEnum.PartiallyShipped,
				mark, cancellationToken).ConfigureAwait(false));
			orders.AddRange(await this.GetModifiedOrdersByStatusAsync(startDateUtc, endDateUtc, NewEggOrderStatusEnum.Shipped,
				mark, cancellationToken).ConfigureAwait(false));
			orders.AddRange(await this.GetModifiedOrdersByStatusAsync(startDateUtc, endDateUtc, NewEggOrderStatusEnum.Invoiced,
				mark, cancellationToken).ConfigureAwait(false));
			orders.AddRange(await this.GetModifiedOrdersByStatusAsync(startDateUtc, endDateUtc, NewEggOrderStatusEnum.Voided,
				mark, cancellationToken).ConfigureAwait(false));

			return orders;
		}

		public abstract GetModifiedOrdersCommand GetModifiedOrdersCommand(NewEggConfig config, NewEggCredentials credentials, string payload);

		internal async Task<IEnumerable<NewEggOrder>> GetModifiedOrdersByStatusAsync(DateTime startDateUtc, DateTime endDateUtc, NewEggOrderStatusEnum orderStatus, 
			Mark mark, CancellationToken token)
		{
			var orders = new List<NewEggOrder>();

			int pageIndex = 1;

			while (true)
			{
				var request = new GetOrderInfoRequest(
					new GetOrderInfoRequestBody(
						pageIndex,
						base.Config.OrdersPageSize,
						new GetOrderInfoRequestCriteria()
						{
							Type = 0,
							Status = (int)orderStatus,
							OrderDateFrom = Misc.ConvertFromUtcToPstStr(startDateUtc),
							OrderDateTo = Misc.ConvertFromUtcToPstStr(endDateUtc)
						}));

				var ordersPageServerResponse = await base.PutAsync(GetModifiedOrdersCommand(base.Config, base.Credentials, request.ToJson()), token, mark, (code, response) => false).ConfigureAwait(false);

				if (ordersPageServerResponse.Error == null)
				{
					var ordersPage = JsonConvert.DeserializeObject<GetOrderInfoResponse>(ordersPageServerResponse.Result);

					if (ordersPage.IsSuccess)
					{
						if (ordersPage.ResponseBody.OrderInfoList != null)
						{
							orders.AddRange(ordersPage.ResponseBody.OrderInfoList.Select(o => o.ToSVOrder()));
							++pageIndex;
						}

						if (ordersPage.ResponseBody.PageInfo.TotalPageCount <= pageIndex)
						{
							break;
						}
					}
					else
					{
						break;
					}
				}
				else
				{
					throw new NewEggException(ordersPageServerResponse.Error.Message);
				}
			}

			return orders.ToArray();
		}
	}
}