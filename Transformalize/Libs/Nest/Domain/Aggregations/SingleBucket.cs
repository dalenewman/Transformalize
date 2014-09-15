using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Aggregations
{
	public class SingleBucket : BucketAggregationBase
	{
		[JsonProperty("doc_count")]
		public long DocCount { get; set; }
	}
}