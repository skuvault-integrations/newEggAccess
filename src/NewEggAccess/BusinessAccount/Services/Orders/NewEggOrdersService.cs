using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NewEggAccess.BusinessAccount.Models.Commands;
using NewEggAccess.Configuration;
using NewEggAccess.Exceptions;
using NewEggAccess.Models.Orders;
using NewEggAccess.Services.Orders;
using NewEggAccess.Shared;
using Newtonsoft.Json;

namespace NewEggAccess.BusinessAccount.Services.Orders
{
	public class NewEggOrdersService : BaseService, INewEggOrdersService
	{
		public NewEggOrdersService( NewEggConfig config, NewEggCredentials credentials ) : base( credentials, config )
		{
		}

		public async Task< IEnumerable< NewEggOrder > > GetModifiedOrdersAsync( DateTime startDateUtc, DateTime endDateUtc, string countryCode, CancellationToken token, Mark mark = null )
		{
			var orders = new List< NewEggOrder >();
			// order status actually cannot be omitted in request body (API error)
			orders.AddRange( await this.GetModifiedOrdersByStatusAsync( startDateUtc, endDateUtc, NewEggOrderStatusEnum.Unshipped, token, mark ).ConfigureAwait( false ) );
			orders.AddRange( await this.GetModifiedOrdersByStatusAsync( startDateUtc, endDateUtc, NewEggOrderStatusEnum.PartiallyShipped, token, mark ).ConfigureAwait( false ) );
			orders.AddRange( await this.GetModifiedOrdersByStatusAsync( startDateUtc, endDateUtc, NewEggOrderStatusEnum.Shipped, token, mark ).ConfigureAwait( false ) );
			orders.AddRange( await this.GetModifiedOrdersByStatusAsync( startDateUtc, endDateUtc, NewEggOrderStatusEnum.Invoiced, token, mark ).ConfigureAwait( false ) );
			orders.AddRange( await this.GetModifiedOrdersByStatusAsync( startDateUtc, endDateUtc, NewEggOrderStatusEnum.Voided, token, mark ).ConfigureAwait( false ) );

			return orders;
		}
		
		private async Task< IEnumerable< NewEggOrder > > GetModifiedOrdersByStatusAsync( DateTime startDateUtc, DateTime endDateUtc, NewEggOrderStatusEnum orderStatus, CancellationToken token, Mark mark = null )
		{
			var orders = new List< NewEggOrder >();

			if ( mark == null )
			{
				mark = Mark.CreateNew();
			}

			int pageIndex = 1;

			while ( true )
			{
				var request = new GetOrderInfoRequest(
					new GetOrderInfoRequestBody(
						pageIndex,
						base.Config.OrdersPageSize,
						new GetOrderInfoRequestCriteria()
						{
							Type = 0,
							Status = (int)orderStatus,
							OrderDateFrom = Misc.ConvertFromUtcToPstStr( startDateUtc ),
							OrderDateTo = Misc.ConvertFromUtcToPstStr( endDateUtc )							
						} ) );

				var ordersPageServerResponse = await base.PutAsync( new GetModifiedOrdersCommand( base.Config, base.Credentials, request.ToJson() ), token, mark, ( code, response ) => false ).ConfigureAwait( false );

				if ( ordersPageServerResponse.Error == null )
				{
					var ordersPage = JsonConvert.DeserializeObject< GetOrderInfoResponse >( ordersPageServerResponse.Result );

					if ( ordersPage.IsSuccess )
					{
						if ( ordersPage.ResponseBody.OrderInfoList != null )
						{
							orders.AddRange( ordersPage.ResponseBody.OrderInfoList.Select( o => o.ToSVOrder() ) );
							++pageIndex;
						}

						if ( ordersPage.ResponseBody.PageInfo.TotalPageCount <= pageIndex )
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
					throw new NewEggException( ordersPageServerResponse.Error.Message );
				}
			}

			return orders.ToArray();
		}
	}
}