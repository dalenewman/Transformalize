using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.Domain.Similarity
{
	[JsonConverter(typeof(SimilaritySettingsConverter))]
	public class SimilaritySettings
	{
		public SimilaritySettings()
		{
			this.CustomSimilarities = new Dictionary<string, SimilarityBase>();
		}

		public string Default { get; set; }

		[JsonConverter(typeof(SimilarityCollectionConverter))]
		public IDictionary<string, SimilarityBase> CustomSimilarities { get; set; }
	}
}
