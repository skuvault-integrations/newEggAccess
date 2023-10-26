using System;
using System.Linq;
using NewEggAccess.Models.Items.Business;
using Newtonsoft.Json;

namespace NewEggAccess.Services.Business.Converters
{
	internal class GetBatchInventoryResponseJsonConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var source = serializer.Deserialize<BatchInventoryResponse>(reader);

			return new Models.Items.BatchInventoryResponse
			{
				ResponseBody = new Models.Items.BatchInventoryResponseBody
				{
					ItemList = source.ResponseBody.ItemList.Select(x => x.ToRegularItemInventory()).ToList()
				}
			};
		}

		public override bool CanRead => true;

		public override bool CanConvert(Type type) => typeof(Models.Items.BatchInventoryResponse).IsAssignableFrom(type);
	}
}