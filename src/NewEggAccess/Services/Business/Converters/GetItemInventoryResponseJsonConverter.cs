using System;
using NewEggAccess.Models.Items.Business;
using Newtonsoft.Json;

namespace NewEggAccess.Services.Business.Converters
{
	internal class GetItemInventoryResponseJsonConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var source = serializer.Deserialize<ItemInventory>(reader);
			return source.ToRegularItemInventory();
		}

		public override bool CanRead => true;

		public override bool CanConvert(Type type) => typeof(Models.Items.ItemInventory).IsAssignableFrom(type);
	}
}