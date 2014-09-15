using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Similarity
{
	public class BM25Similarity : SimilarityBase
	{
		public BM25Similarity()
		{
			this.Type = "BM25";
		}

		[JsonProperty("k1")]
		public double K1 { get; set; }

		[JsonProperty("b")]
		public double B { get; set; }

		[JsonProperty("discount_overlaps")]
		public bool DiscountOverlaps { get; set; }
	}
}
