using System;
using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.DSL
{
	public interface ICustomJsonReader<out T> where T : class, new()
	{
		T FromJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer);
	}
}