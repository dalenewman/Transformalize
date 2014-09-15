using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Aggregations
{
	public class TermsIncludeExclude
	{
		[JsonProperty("pattern")]
		public string Pattern { get; set; }
		[JsonProperty("flags")]
		public string Flags { get; set; }
	}
}