using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Analysis.Tokenizer
{
	/// <summary>
	/// A tokenizer of type uax_url_email which works exactly like the standard tokenizer, but tokenizes emails and urls as single tokens
	/// </summary>
	public class UaxEmailUrlTokenizer : TokenizerBase
    {
		public UaxEmailUrlTokenizer()
        {
			Type = "uax_url_email";
        }

		/// <summary>
		/// The maximum token length. If a token is seen that exceeds this length then it is discarded. Defaults to 255.
		/// </summary>
		[JsonProperty("max_token_length")]
		public int? MaximumTokenLength { get; set; }		
    }
}
