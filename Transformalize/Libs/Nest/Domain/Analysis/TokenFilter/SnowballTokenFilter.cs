using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Analysis.TokenFilter
{
    /// <summary>
	/// A filter that stems words using a Snowball-generated stemmer.
    /// </summary>
    public class SnowballTokenFilter : TokenFilterBase
    {
		public SnowballTokenFilter()
            : base("snowball")
        {

        }

		[JsonProperty("language")]
		public string Language { get; set; }

    }
}