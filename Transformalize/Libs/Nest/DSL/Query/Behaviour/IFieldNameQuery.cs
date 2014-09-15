using Transformalize.Libs.Nest.Domain.Marker;

namespace Transformalize.Libs.Nest.DSL.Query.Behaviour
{
	public interface IFieldNameQuery : IQuery
	{
		PropertyPathMarker GetFieldName();
		void SetFieldName(string fieldName);
	}
}