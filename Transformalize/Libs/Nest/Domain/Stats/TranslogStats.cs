using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Stats
{
	public class TranslogStats
	{
		[JsonProperty(PropertyName = "operations")]
		public long Operations { get; set; }
	}
}
