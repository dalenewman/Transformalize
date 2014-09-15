using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.TermVector
{
    [JsonObject]
    public class TermVector
    {
        public TermVector()
        {
            Terms = new Dictionary<string, TermVectorTerm>();
        }

        [JsonProperty("field_statistics")]
        public FieldStatistics FieldStatistics { get; internal set; }

        [JsonProperty("terms")]
        public IDictionary<string, TermVectorTerm> Terms { get; internal set; }
    }
}
