using System;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Newtonsoft.Json.Converters;

namespace Transformalize.Libs.Nest.Resolvers.Converters
{
	public class UnixDateTimeConverter : DateTimeConverterBase
	{
		private static readonly DateTime EpochUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) ;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			long val;
			if (value is DateTime)
			{
				val = (long)((DateTime)value - EpochUtc).TotalMilliseconds;
			}
			else
			{
				throw new System.Exception("Expected date object value.");
			}

			writer.WriteValue(val);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
										JsonSerializer serializer)
		{
			if (reader.TokenType != JsonToken.Integer)
			{
				throw new System.Exception("Wrong Token Type");
			}

			var time = (long)reader.Value;
			return EpochUtc.AddMilliseconds(time);
		}
	}
}