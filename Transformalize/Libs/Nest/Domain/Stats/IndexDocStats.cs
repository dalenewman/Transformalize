using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Stats
{
	public class IndexDocStats
	{
		[JsonProperty("num_docs")]
		public long NumberOfDocs { get; set; }
		[JsonProperty("max_docs")]
		public long MaximumDocs { get; set; }
		[JsonProperty("deleted_docs")]
		public long DeletedDocs { get; set; }
	}
}
