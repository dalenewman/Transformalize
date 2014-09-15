using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Hit
{
    [JsonObject]
    public class HitsMetaData<T> where T : class
    {
        [JsonProperty("total")]
        public long Total { get; internal set; }

        [JsonProperty("max_score")]
        public double MaxScore { get; internal set; }

        [JsonProperty("hits")]
        public List<IHit<T>> Hits { get; internal set; }
    }
}
