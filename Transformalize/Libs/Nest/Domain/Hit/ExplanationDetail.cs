using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Hit
{
	[JsonObject]
	public class ExplanationDetail
	{
		[JsonProperty(PropertyName = "value")]
		public float Value { get; internal set; }
		[JsonProperty(PropertyName = "description")]
		public string Description { get; internal set; }
        [JsonProperty(PropertyName = "details")]
        public IEnumerable<ExplanationDetail> Details { get; set; }
	}
}
