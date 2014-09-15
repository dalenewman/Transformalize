using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Similarity
{
	public class LMDirichletSimilarity : SimilarityBase
	{
		public LMDirichletSimilarity()
		{
			this.Type = "LMDirichlet";
		}

		[JsonProperty("mu")]
		public int Mu { get; set; }
	}
}
