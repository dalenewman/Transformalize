using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Stats
{
	[JsonObject]
	public class Stats
	{
		[JsonProperty(PropertyName = "primaries")]
		public StatsContainer Primaries { get; set; }
		[JsonProperty(PropertyName = "total")]
		public StatsContainer Total { get; set; }
	}
}
