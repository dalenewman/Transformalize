using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Stats
{
	[JsonObject]
	public class ShardHealthStats
	{
		[JsonProperty(PropertyName = "status")]
		public string Status { get; set; }
		[JsonProperty(PropertyName = "primary_active")]
		public bool PrimaryActive { get; set; }
		[JsonProperty(PropertyName = "active_shards")]
		public int ActiveShards { get; set; }
		[JsonProperty(PropertyName = "relocating_shards")]
		public int RelocatingShards { get; set; }
		[JsonProperty(PropertyName = "initializing_shards")]
		public int InitializingShards { get; set; }
		[JsonProperty(PropertyName = "unassigned_shards")]
		public int UnassignedShards { get; set; }
	}
}
