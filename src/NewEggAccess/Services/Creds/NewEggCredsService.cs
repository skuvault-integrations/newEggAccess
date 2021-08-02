using System;
using System.Threading;
using System.Threading.Tasks;
using NewEggAccess.Configuration;
using NewEggAccess.Exceptions;
using NewEggAccess.Models.Commands;
using NewEggAccess.Models.Orders;
using NewEggAccess.Shared;

namespace NewEggAccess.Services.Creds
{
	public class NewEggCredsService : BaseService, INewEggCredsService
	{
		public NewEggCredsService( NewEggConfig config, NewEggCredentials credentials ) : base( credentials, config )
		{
		}

		public async Task<bool> AreNewEggCredentialsValid( CancellationToken token, Mark mark = null )
		{
			if ( mark == null )
			{
				mark = Mark.CreateNew();
			}

			int pageIndex = 1;
			DateTime startDateUtc = DateTime.UtcNow;
			DateTime endDateUtc = startDateUtc.AddMinutes( 1 );

			var request = new GetOrderInfoRequest(
					new GetOrderInfoRequestBody(
						pageIndex,
						base.Config.OrdersPageSize,
						new GetOrderInfoRequestCriteria()
						{
							Type = 0,
							Status = ( int )NewEggOrderStatusEnum.Unshipped,
							OrderDateFrom = Misc.ConvertFromUtcToPstStr( startDateUtc ),
							OrderDateTo = Misc.ConvertFromUtcToPstStr( endDateUtc )							
						}));

			var ordersPageServerResponse = await base.PutAsync( new GetModifiedOrdersCommand( base.Config, base.Credentials, request.ToJson() ), token, mark, ( code, response ) => false ).ConfigureAwait( false );

			if ( ordersPageServerResponse.Error == null )
			{
				return true;
			}
			else
			{
				throw new NewEggException( ordersPageServerResponse.Error.Message );
			}
		}		
	}
}