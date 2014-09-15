using Transformalize.Libs.Nest.Domain.Marker;

namespace Transformalize.Libs.Nest.DSL.Query.SubDescriptors
{
	public class ExternalFieldDeclaration : IExternalFieldDeclaration
	{
		public IndexNameMarker Index { get; set; }
		public TypeNameMarker Type { get; set; }
		public string Id { get; set; }
		public PropertyPathMarker Path { get; set; }
	}
}