using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Hit;

namespace Transformalize.Libs.Nest.Resolvers.Converters
{
	public class ShardsSegmentConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{

		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
										JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.StartArray)
			{
				var list = new List<ShardsSegment>();
				serializer.Populate(reader, list);
				return list.First();
			}

			var o = new ShardsSegment();
			serializer.Populate(reader, o);
			return o;
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(ShardsSegment);
		}
	}
}