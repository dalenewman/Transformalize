using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Marker;

namespace Transformalize.Libs.Nest.Domain.Alias
{
	public class AliasRemoveOperation
	{
		[JsonProperty("index")]
		public IndexNameMarker Index { get; set; }
		[JsonProperty("alias")]
		public string Alias { get; set; }
	}
}