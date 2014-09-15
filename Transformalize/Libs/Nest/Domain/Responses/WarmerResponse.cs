using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Mapping.Types;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.Domain.Responses
{
	public interface IWarmerResponse : IResponse
	{
		Dictionary<string, Dictionary<string, WarmerMapping>> Indices { get; }
	}

	[JsonObject]
	public class WarmerResponse : BaseResponse, IWarmerResponse
	{
		[JsonConverter(typeof(DictionaryKeysAreNotPropertyNamesJsonConverter))]
		public Dictionary<string, Dictionary<string, WarmerMapping>> Indices { get; internal set; }

	}
}
