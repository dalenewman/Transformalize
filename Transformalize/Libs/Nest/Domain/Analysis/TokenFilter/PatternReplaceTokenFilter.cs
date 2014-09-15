using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Analysis.TokenFilter
{
    public class PatternReplaceTokenFilter : TokenFilterBase
    {
        public PatternReplaceTokenFilter() : base("pattern_replace")
        {
        }

        [JsonProperty("pattern")]
        public string Pattern { get; set; }

        [JsonProperty("replacement")]
        public string Replacement { get; set; }
    }
}