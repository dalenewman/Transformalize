using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Stats
{
	[JsonObject]
	public class StoreStats
	{
		[JsonProperty(PropertyName = "size")]
		public string Size { get; set; }
		[JsonProperty(PropertyName = "size_in_bytes")]
		public double SizeInBytes { get; set; }
	}

}
