using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Facets
{
    [JsonObject]
    public class HistogramFacet : Facet, IFacet<HistogramFacetItem>
    {
        [JsonProperty("entries")]
        public IEnumerable<HistogramFacetItem> Items { get; internal set; }
    }
    public class HistogramFacetItem : FacetItem
    {
        [JsonProperty("key")]
        public double Key { get; set; }
    }

}