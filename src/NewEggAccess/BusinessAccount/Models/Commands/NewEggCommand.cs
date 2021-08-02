using CuttingEdge.Conditions;
using NewEggAccess.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace NewEggAccess.BusinessAccount.Models.Commands
{
	public abstract class NewEggCommand
	{
		public const string GetItemInventoryServiceUrl = "/contentmgmt/item/inventory";
		public const string UpdateItemInventoryServiceUrl = "/contentmgmt/item/inventoryandprice";
		public const string SubmitFeedServiceUrl = "/datafeedmgmt/feeds/submitfeed";
		public const string GetFeedStatusServiceUrl = "/datafeedmgmt/feeds/status";
		public const string GetModifiedOrdersServiceUrl = "/ordermgmt/order/orderinfo";

		public const string ApiVersion = "305";

		public NewEggConfig Config { get; private set; }
		public NewEggCredentials Credentials { get; private set; }
		public string RelativeUrl { get; private set; }

		public string Url
		{
			get 
			{
				var urlParameters = string.Join( "&", this._urlParameters.Select( pair => $"{ pair.Key }={ pair.Value }" ) );
				return $"{ this.Config.ApiBaseUrl }{ "/b2b" }{ this.RelativeUrl }?{ urlParameters }"; 
			}
		}
		public string Payload { get; protected set; }

		private Dictionary< string, string > _urlParameters { get; set; }

		protected NewEggCommand( NewEggConfig config, NewEggCredentials credentials, string relativeUrl )
		{
			Condition.Requires( config, "config" ).IsNotNull();
			Condition.Requires( credentials, "credentials" ).IsNotNull();

			this.Config = config;
			this.Credentials = credentials;
			this.RelativeUrl = relativeUrl;
			this._urlParameters = new Dictionary< string, string >
			{
				{ "sellerId", this.Credentials.SellerId }
			};
		}
		
		protected void AddUrlParameter( string name, string value )
		{
			Condition.Requires( name, "name" ).IsNotNullOrWhiteSpace();
			Condition.Requires( value, "value" ).IsNotNullOrWhiteSpace();

			this._urlParameters.Add( name, value );
		}
	}
}