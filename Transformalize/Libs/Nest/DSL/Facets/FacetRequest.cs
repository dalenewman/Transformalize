using Transformalize.Libs.Nest.Domain.Marker;
using Transformalize.Libs.Nest.DSL.Filter;

namespace Transformalize.Libs.Nest.DSL.Facets
{
	public interface IFacetRequest
	{
		bool? Global { get; set; }
		IFilterContainer FacetFilter { get; set; }
		PropertyPathMarker Nested { get; set; }
	}
	public abstract class FacetRequest : IFacetRequest
	{
		public bool? Global { get; set; }
		public IFilterContainer FacetFilter { get; set; }
		public PropertyPathMarker Nested { get; set; }
	}
}