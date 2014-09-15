using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Facets
{
	public abstract class FacetItem
	{
		[JsonProperty("count")]
		public virtual long Count { get; internal set; }
	}
}