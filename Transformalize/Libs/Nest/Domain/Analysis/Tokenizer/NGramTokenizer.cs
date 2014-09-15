using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Analysis.Tokenizer
{
	/// <summary>
	/// A tokenizer of type nGram.
	/// </summary>
	public class NGramTokenizer : TokenizerBase
    {
		public NGramTokenizer()
        {
            Type = "nGram";
        }

		[JsonProperty("min_gram")]
		public int? MinGram { get; set; }

		[JsonProperty("max_gram")]
		public int? MaxGram { get; set; }

        [JsonProperty("token_chars")]
        public IList<string> TokenChars { get; set; }
    }
}