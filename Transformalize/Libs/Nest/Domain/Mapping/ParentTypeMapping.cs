using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Marker;

namespace Transformalize.Libs.Nest.Domain.Mapping
{
    public class ParentTypeMapping
    {
        [JsonProperty("type")]
		public TypeNameMarker Type { get; set; }
    }
}
