using System;
using NewEggAccess.Models.Business.Items;
using Newtonsoft.Json;

namespace NewEggAccess.Services.Business.Converters
{
	public class GetUpdateInventoryResponseJsonConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var source = serializer.Deserialize<UpdateItemInventoryResponse>(reader);

			return new Models.Items.UpdateItemInventoryResponse
			{
				ItemNumber = source.ItemNumber,
				SellerId = source.SellerId,
				SellerPartNumber = source.SellerPartNumber,
				InventoryList = new Models.Items.UpdateItemInventory[]
				{
					new Models.Items.UpdateItemInventory
					{
						WarehouseLocation = "USA",
						AvailableQuantity = source.AvailableQuantity
					}
				}
			};
		}

		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanConvert(Type type)
		{
			return typeof(Models.Items.UpdateItemInventoryResponse).IsAssignableFrom(type);
		}
	}
}