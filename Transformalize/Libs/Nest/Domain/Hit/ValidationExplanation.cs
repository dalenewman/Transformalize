using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Hit
{
	[JsonObject]
	public class ValidationExplanation
	{
		[JsonProperty(PropertyName = "index")]
		public string Index { get; internal set; }
		[JsonProperty(PropertyName = "description")]
		public bool Valid { get; internal set; }
		[JsonProperty(PropertyName = "error")]
		public string Error { get; internal set; }
		
	}
}
