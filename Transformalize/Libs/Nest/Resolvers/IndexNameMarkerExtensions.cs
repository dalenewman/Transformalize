using Transformalize.Libs.Nest.Domain.Connection;
using Transformalize.Libs.Nest.Domain.Marker;
using Transformalize.Libs.Nest.Extensions;

namespace Transformalize.Libs.Nest.Resolvers
{
	public static class IndexNameMarkerExtensions
	{
		
		public static string Resolve(this IndexNameMarker marker, IConnectionSettingsValues connectionSettings)
		{
			if (marker == null)
				return null;
			connectionSettings.ThrowIfNull("connectionSettings");

			if (marker.Type == null)
				return marker.Name;
			return new IndexNameResolver(connectionSettings).GetIndexForType(marker.Type);
		}
	}
}