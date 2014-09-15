using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Similarity
{
	public class LMJelinekSimilarity : SimilarityBase
	{
		public LMJelinekSimilarity()
		{
			this.Type = "LMJelinekMercer";
		}

		[JsonProperty("lambda")]
		public double Lambda { get; set; }
	}
}
