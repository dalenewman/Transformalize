using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Similarity
{
	public class DefaultSimilarity : SimilarityBase
	{
		public DefaultSimilarity()
		{
			this.Type = "default";
		}

		[JsonProperty("discount_overlaps")]
		public bool DiscountOverlaps { get; set; }
	}
}
