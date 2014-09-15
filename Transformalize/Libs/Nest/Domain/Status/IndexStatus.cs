using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Stats;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.Domain.Status
{
    public class IndexStatus
    {
        [JsonProperty(PropertyName = "index")]
        public IndexSizeStats Index { get; set; }

        [JsonProperty(PropertyName = "translog")]
        public TranslogStats Translog { get; set; }

        [JsonProperty(PropertyName = "docs")]
        public IndexDocStats IndexDocs { get; set; }
        
        [JsonProperty(PropertyName = "merges")]
        public MergesStats Merges { get; set; }
        
        [JsonProperty(PropertyName = "refresh")]
        public RefreshStats Refresh { get; set; }

        [JsonProperty(PropertyName = "flush")]
        public FlushStats Flush { get; set; }
    
        [JsonProperty(PropertyName = "shards")]
        [JsonConverter(typeof(DictionaryKeysAreNotPropertyNamesJsonConverter))]
        public Dictionary<string, dynamic> Shards { get; internal set; }
    

    }
}
