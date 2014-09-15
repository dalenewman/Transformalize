using Transformalize.Libs.Nest.Domain.Marker;

namespace Transformalize.Libs.Nest.Domain.Mapping.Types
{
	public interface IElasticType 
	{
		PropertyNameMarker Name { get; set; }
		TypeNameMarker Type { get; }
	}
}
