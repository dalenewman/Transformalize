using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.Domain.Hit
{
    [JsonObject]
    [JsonConverter(typeof(ShardsSegmentConverter))]
    public class ShardsSegment
    {
        [JsonProperty(PropertyName = "num_committed_segments")]
        public int CommittedSegments { get; internal set; }

        [JsonProperty(PropertyName = "num_search_segments")]
        public int SearchSegments { get; internal set; }

        [JsonProperty(PropertyName = "routing")]
        public ShardSegmentRouting Routing { get; internal set; }

        [JsonProperty]
		[JsonConverter(typeof(DictionaryKeysAreNotPropertyNamesJsonConverter))]
		public Dictionary<string, Segment> Segments { get; internal set; }

    }
}