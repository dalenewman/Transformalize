using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Similarity
{
	public class DFRSimilarity : SimilarityBase
	{
		public DFRSimilarity()
		{
			this.Type = "DFR";
		}

		[JsonProperty("basic_model")]
		public string BasicModel { get; set; }

		[JsonProperty("after_effect")]
		public string AfterEffect { get; set; }
	}
}