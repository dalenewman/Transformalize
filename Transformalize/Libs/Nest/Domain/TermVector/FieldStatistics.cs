using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.TermVector
{
    [JsonObject]
    public class FieldStatistics
    {
        [JsonProperty("doc_count")]
        public int DocumentCount { get; internal set; }

        [JsonProperty("sum_doc_freq")]
        public int SumOfDocumentFrequencies { get; internal set; }
        
        [JsonProperty("sum_ttf")]
        public int SumOfTotalTermFrequencies { get; internal set; }
    }
}
