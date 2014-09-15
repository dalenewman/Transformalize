using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Hit;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.Domain.Responses
{
	public interface IGlobalStatsResponse : IResponse
	{
		ShardsMetaData Shards { get; }
		Stats.Stats Stats { get; set; }
		Dictionary<string, Stats.Stats> Indices { get; set; }
	}

	[JsonObject]
	public class GlobalStatsResponse : BaseResponse, IGlobalStatsResponse
	{

		[JsonProperty(PropertyName = "_shards")]
		public ShardsMetaData Shards { get; internal set; }

		[JsonProperty(PropertyName = "_all")]
		public Stats.Stats Stats { get; set; }

		[JsonProperty(PropertyName = "indices")]
		[JsonConverter(typeof(DictionaryKeysAreNotPropertyNamesJsonConverter))]
		public Dictionary<string, Stats.Stats> Indices { get; set; }

	}
}