using CuttingEdge.Conditions;
using NewEggAccess.Configuration;
using NewEggAccess.Exceptions;
using NewEggAccess.Models;
using NewEggAccess.Models.Commands;
using NewEggAccess.Shared;
using NewEggAccess.Throttling;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NewEggAccess.Services
{
	public class BaseService
	{
		protected NewEggCredentials Credentials { get; private set; }
		protected NewEggConfig Config { get; private set; }
		private Throttler _throttler;
		public IHttpClient HttpClient;

		private Func< string > _additionalLogInfo;
		private const int _tooManyRequestsHttpCode = 429;

		/// <summary>
		///	Extra logging information
		/// </summary>
		public Func< string > AdditionalLogInfo
		{
			get { return this._additionalLogInfo ?? ( () => string.Empty ); }
			set => _additionalLogInfo = value;
		}

		public Throttler Throttler
		{
			get
			{
				return _throttler;
			}
		}

		public BaseService( NewEggCredentials credentials, NewEggConfig config )
		{
			Condition.Requires( credentials, "credentials" ).IsNotNull();
			Condition.Requires( config, "config" ).IsNotNull();

			this.Credentials = credentials;
			this.Config = config;

			this._throttler = new Throttler( config.ThrottlingOptions.MaxRequestsPerTimeInterval, config.ThrottlingOptions.TimeIntervalInSec );
			this.HttpClient = new DefaultHttpClient();
			this.HttpClient.SetAcceptHeader( new MediaTypeWithQualityHeaderValue( "application/json" ) );

			SetAuthorizationHeaders();
		}

		private void SetAuthorizationHeaders()
		{
			this.HttpClient.SetRequestHeader( "Authorization", this.Credentials.ApiKey );
			this.HttpClient.SetRequestHeader( "SecretKey", this.Credentials.SecretKey );
		}

		protected Task< ServerResponse > GetAsync( string url, CancellationToken cancellationToken, Mark mark, Func< HttpStatusCode, ErrorResponse, bool > ignoreError )
		{
			if ( cancellationToken.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( url, mark, additionalInfo: this.AdditionalLogInfo() );
				throw new NewEggException( string.Format( "{0}. Task was cancelled", exceptionDetails ) );
			}

			return this.ThrottleRequest( url, string.Empty, mark, async ( token ) =>
			{
				var httpResponse = await HttpClient.GetAsync( url ).ConfigureAwait( false );
				var content = await httpResponse.ReadContentAsStringAsync().ConfigureAwait( false );

				this.SaveAndLogRateLimits( httpResponse, CreateMethodCallInfo( url, mark, additionalInfo: this.AdditionalLogInfo() ) );
				var errorResponse = ThrowIfError( httpResponse, content, ignoreError );

				return new ServerResponse() { Result = content, Error = errorResponse };
			}, cancellationToken );
		}
		
		protected Task< ServerResponse > PutAsync( NewEggCommand command, CancellationToken cancellationToken, Mark mark, Func< HttpStatusCode, ErrorResponse, bool > ignoreError )
		{
			if ( cancellationToken.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( command.Url, mark, additionalInfo: this.AdditionalLogInfo() );
				throw new NewEggException( string.Format( "{0}. Task was cancelled", exceptionDetails ) );
			}

			return this.ThrottleRequest( command.Url, command.Payload, mark, async ( token ) =>
			{
				var payload = new StringContent( command.Payload, Encoding.UTF8, "application/json" );				
				// NewEgg service responds only on application/json without charset specified
				payload.Headers.ContentType = MediaTypeHeaderValue.Parse( "application/json" );
				var httpResponse = await HttpClient.PutAsync( command.Url, payload, token ).ConfigureAwait( false );
				var content = await httpResponse.ReadContentAsStringAsync().ConfigureAwait( false );

				this.SaveAndLogRateLimits( httpResponse, CreateMethodCallInfo( command.Url, mark, additionalInfo: this.AdditionalLogInfo() ) );
				var errorResponse = ThrowIfError( httpResponse, content, ignoreError );

				return new ServerResponse() { Result = content, Error = errorResponse };
				
			}, cancellationToken );
		}

		protected Task< ServerResponse > PostAsync( NewEggCommand command, CancellationToken cancellationToken, Mark mark, Func< HttpStatusCode, ErrorResponse, bool > ignoreError )
		{
			if ( cancellationToken.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( command.Url, mark, additionalInfo: this.AdditionalLogInfo() );
				throw new NewEggException( string.Format( "{0}. Task was cancelled", exceptionDetails ) );
			}

			return this.ThrottleRequest( command.Url, command.Payload, mark, async ( token ) =>
			{
				var payload = new StringContent( command.Payload, Encoding.UTF8, "application/json" );
				// NewEgg service responds only on application/json without charset specified
				payload.Headers.ContentType = MediaTypeHeaderValue.Parse( "application/json" );
				var httpResponse = await HttpClient.PostAsync( command.Url, payload, token ).ConfigureAwait( false );
				var content = await httpResponse.ReadContentAsStringAsync().ConfigureAwait( false );

				this.SaveAndLogRateLimits( httpResponse, CreateMethodCallInfo( command.Url, mark, additionalInfo: this.AdditionalLogInfo() ) );
				var errorResponse = ThrowIfError( httpResponse, content, ignoreError );

				return new ServerResponse() { Result = content, Error = errorResponse };
			}, cancellationToken );
		}

		protected ErrorResponse ThrowIfError( IHttpResponseMessage response, string message, Func< HttpStatusCode, ErrorResponse, bool > ignoreError )
		{
			ErrorResponse errorResponse = null;
			HttpStatusCode responseStatusCode = response.StatusCode;

			if ( response.IsSuccessStatusCode )
				return errorResponse;

			try
			{
				if ( message != null )
				{
					var errors = JsonConvert.DeserializeObject< ErrorResponse[] >( message );
					errorResponse = errors.FirstOrDefault();
				}
			}
			catch { }

			// ignore errors which is not errors actually like item was not found
			if ( ignoreError( response.StatusCode, errorResponse ) )
			{
				return errorResponse;
			}

			if ( responseStatusCode == HttpStatusCode.Unauthorized )
			{
				throw new NewEggUnauthorizedException( message );
			}
			else if ( responseStatusCode == HttpStatusCode.InternalServerError
					|| responseStatusCode == HttpStatusCode.BadRequest )
			{
				throw new NewEggException( message );
			}
			else if ( (int)responseStatusCode == _tooManyRequestsHttpCode )
			{
				throw new NewEggRateLimitsExceeded( message );
			}

			throw new NewEggException( message );
		}

		private void SaveAndLogRateLimits( IHttpResponseMessage response, string info )
		{
			var limits = GetRateLimit( response );

			if ( limits != null )
			{
				this._throttler.RateLimit.Limit = limits.Limit;
				this._throttler.RateLimit.Remaining = limits.Remaining;
				this._throttler.RateLimit.ResetTime = limits.ResetTime;

				NewEggLogger.LogTrace( String.Format( "{0}, Total calls: {1}, Remaining calls: {2}, Reset time: {3}", info, limits.Limit, limits.Remaining, limits.ResetTime ) );
			}
		}

		private NewEggRateLimit GetRateLimit( IHttpResponseMessage response )
		{
			var rateLimit = response.GetHeaderValue( "X-RateLimit-Limit" );
			var rateRemaining = response.GetHeaderValue( "X-RateLimit-Remaining" );
			var rateResetTime = response.GetHeaderValue( "X-ratelimit-resettime" );

			if ( !string.IsNullOrWhiteSpace( rateLimit )
				&& !string.IsNullOrWhiteSpace( rateRemaining )
				&& !string.IsNullOrWhiteSpace( rateResetTime ) )
			{
				return new NewEggRateLimit( rateLimit, rateRemaining, rateResetTime );
			}

			return null;
		}

		protected Task< T > ThrottleRequest< T >( string url, string payload, Mark mark, Func< CancellationToken, Task< T > > processor, CancellationToken token )
		{
			return Throttler.ExecuteAsync( () =>
			{
				return new ActionPolicy( Config.NetworkOptions.RetryAttempts, Config.NetworkOptions.DelayBetweenFailedRequestsInSec, Config.NetworkOptions.DelayFailRequestRate )
					.ExecuteAsync( async () =>
					{
						using( var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource( token ) )
						{
							NewEggLogger.LogStarted( this.CreateMethodCallInfo( url, mark, payload: payload, additionalInfo: this.AdditionalLogInfo() ) );
							linkedTokenSource.CancelAfter( Config.NetworkOptions.RequestTimeoutMs );

							var result = await processor( linkedTokenSource.Token ).ConfigureAwait( false );

							NewEggLogger.LogEnd( this.CreateMethodCallInfo( url, mark, methodResult: result.ToJson(), additionalInfo: this.AdditionalLogInfo() ) );

							return result;
						}
					}, 
					( exception, timeSpan, retryCount ) =>
					{
						string retryDetails = CreateMethodCallInfo( url, mark, additionalInfo: this.AdditionalLogInfo() );
						NewEggLogger.LogTraceRetryStarted( timeSpan.Seconds, retryCount, retryDetails );
					},
					() => CreateMethodCallInfo( url, mark, additionalInfo: this.AdditionalLogInfo() ),
					NewEggLogger.LogTraceException );
			} );
		}

		/// <summary>
		///	Creates method calling detailed information
		/// </summary>
		/// <param name="url">Absolute path to service endpoint</param>
		/// <param name="mark">Unique stamp to track concrete method</param>
		/// <param name="errors">Errors</param>
		/// <param name="methodResult">Service endpoint raw result</param>
		/// <param name="payload">Method payload (POST)</param>
		/// <param name="additionalInfo">Extra logging information</param>
		/// <param name="memberName">Method name</param>
		/// <returns></returns>
		protected string CreateMethodCallInfo( string url = "", Mark mark = null, string errors = "", string methodResult = "", string additionalInfo = "", string payload = "", [ CallerMemberName ] string memberName = "" )
		{
			string serviceEndPoint = null;
			string requestParameters = null;

			if ( !string.IsNullOrEmpty( url ) )
			{
				Uri uri = new Uri( url.Contains( Config.ApiBaseUrl ) ? url : Config.ApiBaseUrl + url );

				serviceEndPoint = uri.LocalPath;
				requestParameters = uri.Query;
			}

			var str = string.Format(
				"{{MethodName: {0}, Mark: '{1}', ServiceEndPoint: '{2}', {3} {4}{5}{6}{7}}}",
				memberName,
				mark ?? Mark.Blank(),
				string.IsNullOrWhiteSpace( serviceEndPoint ) ? string.Empty : serviceEndPoint,
				string.IsNullOrWhiteSpace( requestParameters ) ? string.Empty : ", RequestParameters: " + requestParameters,
				string.IsNullOrWhiteSpace( errors ) ? string.Empty : ", Errors:" + errors,
				string.IsNullOrWhiteSpace( methodResult ) ? string.Empty : ", Result:" + methodResult,
				string.IsNullOrWhiteSpace( payload ) ? string.Empty : ", Payload:" + payload,
				string.IsNullOrWhiteSpace( additionalInfo ) ? string.Empty : ", " + additionalInfo
			);
			return str;
		}
	}
}