using Transformalize.Libs.Nest.Domain.Marker;

namespace Transformalize.Libs.Nest.DSL.Filter
{
	public interface IFieldNameFilter : IFilter
	{
		PropertyPathMarker Field { get; set; }
	}
}