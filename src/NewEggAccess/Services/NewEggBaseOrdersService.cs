﻿using System;
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
	/// <summary>
	/// Service for orders sync 
	/// (common for NewEgg Regular and Business Accounts)
	/// </summary>
	public abstract class NewEggBaseOrdersService : BaseService, INewEggOrdersService
	{
		public NewEggBaseOrdersService(NewEggConfig config, NewEggCredentials credentials) : base(credentials, config)
		{
		}

		/// <summary>
		/// Getting a list of modified orders within the period
		/// </summary>
		/// <param name="startDateUtc"></param>
		/// <param name="endDateUtc"></param>
		/// <param name="countryCode"></param>
		/// <param name="mark"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Getting of unshipped orders for a short time period
		/// It uses for validation etsy credentials 
		/// </summary>
		/// <param name="mark"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		/// <exception cref="NewEggException"></exception>
		public async Task<bool> TryGetOrdersAsync(Mark mark, CancellationToken cancellationToken)
		{
			int pageIndex = 1;
			DateTime startDateUtc = DateTime.UtcNow;
			DateTime endDateUtc = startDateUtc.AddMinutes(1);

			var ordersPageServerResponse = await GetOrdersAsync(pageIndex, NewEggOrderStatusEnum.Unshipped, startDateUtc, endDateUtc,
				mark, cancellationToken).ConfigureAwait(false);

			if (ordersPageServerResponse.Error == null)
			{
				return true;
			}
			else
			{
				throw new NewEggException(ordersPageServerResponse.Error.Message);
			}
		}

		public abstract GetModifiedOrdersCommand GetModifiedOrdersCommand(NewEggConfig config, NewEggCredentials credentials, string payload);

		internal async Task<IEnumerable<NewEggOrder>> GetModifiedOrdersByStatusAsync(DateTime startDateUtc, DateTime endDateUtc, NewEggOrderStatusEnum orderStatus,
			Mark mark, CancellationToken cancellationToken)
		{
			var orders = new List<NewEggOrder>();

			int pageIndex = 1;

			while (true)
			{
				var ordersPageServerResponse = await GetOrdersAsync(pageIndex, orderStatus, startDateUtc, endDateUtc,
					mark, cancellationToken).ConfigureAwait(false);

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

		private async Task<Models.ServerResponse> GetOrdersAsync(int pageIndex, NewEggOrderStatusEnum orderStatus, DateTime startDateUtc, DateTime endDateUtc, Mark mark, CancellationToken cancellationToken)
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

			var ordersPageServerResponse = await base.PutAsync(GetModifiedOrdersCommand(base.Config, base.Credentials, request.ToJson()), 
				cancellationToken, mark, (code, response) => false).ConfigureAwait(false);
			return ordersPageServerResponse;
		}
	}
}