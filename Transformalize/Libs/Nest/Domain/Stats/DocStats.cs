using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Stats
{
	[JsonObject]
	public class DocStats
	{
		[JsonProperty(PropertyName = "count")]
		public long Count { get; set; }
		[JsonProperty(PropertyName = "deleted")]
		public long Deleted { get; set; }
	}
}
