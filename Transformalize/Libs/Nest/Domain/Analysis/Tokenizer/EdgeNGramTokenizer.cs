using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Analysis.Tokenizer
{
	/// <summary>
	/// A tokenizer of type edgeNGram.
	/// </summary>
	public class EdgeNGramTokenizer : TokenizerBase
    {
		public EdgeNGramTokenizer()
        {
            Type = "edgeNGram";
        }

		[JsonProperty("min_gram")]
		public int? MinGram { get; set; }

		[JsonProperty("max_gram")]
		public int? MaxGram { get; set; }

		[JsonProperty("side")]
		public string Side { get; set; }

        [JsonProperty("token_chars")]
        public IList<string> TokenChars { get; set; }
    }
}