using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Analysis.TokenFilter
{
    /// <summary>
	/// A filter that stems words (similar to snowball, but with more options).
    /// </summary>
    public class StemmerTokenFilter : TokenFilterBase
    {
		public StemmerTokenFilter()
            : base("stemmer")
        {

        }

		[JsonProperty("language")]
		public string Language { get; set; }

    }
}