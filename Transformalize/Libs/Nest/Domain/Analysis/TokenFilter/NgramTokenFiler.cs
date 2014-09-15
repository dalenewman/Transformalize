using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Analysis.TokenFilter
{
	/// <summary>
	/// A token filter of type nGram.
	/// </summary>
    public class NgramTokenFilter : TokenFilterBase
    {
        public NgramTokenFilter()
            : base("nGram")
        {
        }

        [JsonProperty("min_gram")]
        public int? MinGram { get; set; }

        [JsonProperty("max_gram")]
        public int? MaxGram { get; set; }
    }
}