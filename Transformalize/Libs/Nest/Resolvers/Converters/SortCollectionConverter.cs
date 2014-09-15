using System;
using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Marker;
using Transformalize.Libs.Nest.DSL.Search;

namespace Transformalize.Libs.Nest.Resolvers.Converters
{
	public class SortCollectionConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return typeof(IList<KeyValuePair<PropertyPathMarker, ISort>>).IsAssignableFrom(objectType);
		}

		public override bool CanRead
		{
			get { return false; }
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return new InvalidOperationException();
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteStartArray();
			var sortItems = value as IList<KeyValuePair<PropertyPathMarker, ISort>>;
			foreach (var item in sortItems)
			{
				writer.WriteStartObject();
				var contract = serializer.ContractResolver as SettingsContractResolver;
				var fieldName = contract.Infer.PropertyPath(item.Key);
				writer.WritePropertyName(fieldName);
				serializer.Serialize(writer, item.Value);
				writer.WriteEndObject();
			}
			writer.WriteEndArray();
		}
	}
}
