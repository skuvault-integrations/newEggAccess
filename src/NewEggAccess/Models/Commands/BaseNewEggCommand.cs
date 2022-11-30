using CuttingEdge.Conditions;
using NewEggAccess.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace NewEggAccess.Models.Commands
{
	public abstract class BaseNewEggCommand
	{
		public abstract string RelativeUrl { get; }
		public NewEggConfig Config { get; private set; }
		public NewEggCredentials Credentials { get; private set; }		
		public string Payload { get; private set; }

		protected Dictionary<string, string> _urlParameters { get; set; }

		public string Url
		{
			get
			{
				var urlParameters = string.Join("&", this._urlParameters.Select(pair => $"{ pair.Key }={ pair.Value }"));
				return $"{ this.Config.ApiBaseUrl }{ this.GetPlatformUrl(this.Config.Platform) }{ this.RelativeUrl }?{ urlParameters }";
			}
		}

		protected BaseNewEggCommand(NewEggConfig config, NewEggCredentials credentials, string payload)
		{
			Condition.Requires(payload, "payload").IsNotNullOrWhiteSpace();
			Condition.Requires(config, "config").IsNotNull();
			Condition.Requires(credentials, "credentials").IsNotNull();

			this.Payload = payload;
			this.Config = config;
			this.Credentials = credentials;
			this._urlParameters = new Dictionary<string, string>
			{
				{ "sellerId", this.Credentials.SellerId }
			};
		}

		protected void AddUrlParameter(string name, string value)
		{
			Condition.Requires(name, "name").IsNotNullOrWhiteSpace();
			Condition.Requires(value, "value").IsNotNullOrWhiteSpace();
			this._urlParameters.Add(name, value);
		}

		private string GetPlatformUrl(NewEggPlatform platform)
		{
			switch (platform)
			{
				case NewEggPlatform.NewEggBusiness:
					{
						return "/b2b";
					}
				case NewEggPlatform.NewEggCA:
					{
						return "/can";
					}
				default:
					{
						return string.Empty;
					}
			}
		}
	}
}