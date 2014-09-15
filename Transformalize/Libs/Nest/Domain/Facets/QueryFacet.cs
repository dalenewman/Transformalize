using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Facets
{
		public class QueryFacet : Facet
    {
        [JsonProperty(PropertyName = "count")]
        public long Count { get; internal set; }
    }
}
