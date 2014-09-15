using Transformalize.Libs.Nest.Domain.Marker;
using Transformalize.Libs.Nest.Extensions;

namespace Transformalize.Libs.Nest.Resolvers
{
	public static class TypeNameMarkerExtensions
	{
		
		public static bool IsConditionless(this TypeNameMarker marker)
		{
			return marker == null || marker.Name.IsNullOrEmpty() && marker.Type == null;
		}
	}
}