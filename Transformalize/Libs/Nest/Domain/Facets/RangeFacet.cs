using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Facets
{
	[JsonObject]
	public class RangeFacet : Facet, IFacet<Range>
	{
		[JsonProperty("ranges")]
		public IEnumerable<Range> Items { get; internal set; }

	}
	[JsonObject]
	public class Range : FacetItem
	{
		[JsonProperty(PropertyName = "to")]
		public double? To { get; internal set; }

		[JsonProperty(PropertyName = "from")]
		public double? From { get; internal set; }

		[JsonProperty(PropertyName = "min")]
		public double Min { get; internal set; }

		[JsonProperty(PropertyName = "max")]
		public double Max { get; internal set; }

		[JsonProperty(PropertyName = "total_count")]
		public long TotalCount { get; internal set; }

		[JsonProperty(PropertyName = "total")]
		public double Total { get; internal set; }

		[JsonProperty(PropertyName = "mean")]
		public double Mean { get; internal set; }
	}
}