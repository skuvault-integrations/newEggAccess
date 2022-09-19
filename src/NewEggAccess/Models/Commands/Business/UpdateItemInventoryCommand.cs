﻿using NewEggAccess.Configuration;

namespace NewEggAccess.Models.Commands.Business
{
	public class UpdateItemInventoryCommand : BaseNewEggCommand
	{
		public UpdateItemInventoryCommand(NewEggConfig config, NewEggCredentials credentials, string payload) : base(config, credentials, payload) { }

		public override string RelativeUrl => "/contentmgmt/item/inventoryandprice";
	}
}