using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Similarity
{
	public class IBSimilarity : SimilarityBase
	{
		public IBSimilarity()
		{
			this.Type = "IB";
		}

		[JsonProperty("distribution")]
		public string Distribution { get; set; }

		[JsonProperty("lambda")]
		public string Lambda { get; set; }
	}
}
