using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Hit
{
    [JsonObject]
    public class ShardSegmentRouting
    {
        [JsonProperty(PropertyName = "state")]
        public string State { get; internal set; }

        [JsonProperty(PropertyName = "primary")]
        public bool Primary { get; internal set; }

        [JsonProperty(PropertyName = "node")]
        public string Node { get; internal set; }
    }
}