using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.Domain.Hit
{
    [JsonObject]
    public class IndexSegment
    {
        [JsonProperty(PropertyName = "shards")]
		[JsonConverter(typeof(DictionaryKeysAreNotPropertyNamesJsonConverter))]
		public Dictionary<string, ShardsSegment> Shards { get; internal set; }
    }
}