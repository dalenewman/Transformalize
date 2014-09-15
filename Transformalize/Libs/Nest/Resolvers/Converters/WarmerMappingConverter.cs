using System;
using System.Linq;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Newtonsoft.Json.Linq;
using Transformalize.Libs.Nest.Domain.Mapping.Types;
using Transformalize.Libs.Nest.Domain.Marker;

namespace Transformalize.Libs.Nest.Resolvers.Converters
{
	public class WarmerMappingConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var mapping = (WarmerMapping) value;
			writer.WriteStartObject();

			writer.WritePropertyName("types");
			serializer.Serialize(writer, mapping.Types);

			writer.WritePropertyName("source");
			serializer.Serialize(writer, mapping.Source);

			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
										JsonSerializer serializer)
		{
			var jObject = JObject.Load(reader);
			var types = ((JArray)jObject.Property("types").Value).Values<string>().ToArray()
				.Select(s=>(TypeNameMarker)s);
			var source = jObject.Property("source").Value.ToString();

			return new WarmerMapping
			{
				Types = types, 
			};
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(WarmerMapping);
		}
	}
}