﻿using NewEggAccess.Configuration;
using NewEggAccess.Models.Commands.Business;

namespace NewEggAccess.Services.Regular
{
	/// <summary>
	/// Service for orders sync 
	/// (specific for NewEgg Regular Account - newegg.com)
	/// </summary>
	public class NewEggOrdersService : NewEggBaseOrdersService
	{
		public NewEggOrdersService(NewEggConfig config, NewEggCredentials credentials) : base(config, credentials)
		{
		}

		public override GetModifiedOrdersCommand GetModifiedOrdersCommand(NewEggConfig config, NewEggCredentials credentials, string payload)
		{
			return new GetModifiedOrdersCommand(config, credentials, payload);
		}
	}
}