using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Hit
{
    [JsonObject]
    public class MultiHit<T> where T : class
    {
        [JsonProperty("docs")]
        public IEnumerable<Hit<T>> Hits { get; internal set; }
    }
}
