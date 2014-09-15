using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Stats
{
	[JsonObject]
	public class FlushStats
	{
	
		[JsonProperty(PropertyName = "total")]
		public long Total { get; set; }
		[JsonProperty(PropertyName = "total_time")]
		public string TotalTime { get; set; }
		[JsonProperty(PropertyName = "total_time_in_millis")]
		public double TotalTimeInMilliseconds { get; set; }

	}
}
