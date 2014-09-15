using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Marker;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.DSL.Query.SubDescriptors
{
	[JsonConverter(typeof(ReadAsTypeConverter<ExternalFieldDeclaration>))]
	public interface IExternalFieldDeclaration
	{
		[JsonProperty("index")]
		IndexNameMarker Index { get; set; }
		
		[JsonProperty("type")]
		TypeNameMarker Type { get; set; }
		
		[JsonProperty("id")]
		string Id { get; set; }
		
		[JsonProperty("path")]
		PropertyPathMarker Path { get; set; }
	}
}