using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Newtonsoft.Json.Converters;
using Transformalize.Libs.Nest.Enums;

namespace Transformalize.Libs.Nest.Domain.Mapping.SubMappings
{
	[JsonObject(MemberSerialization.OptIn)]
	public class NormsMapping 
	{
		[JsonProperty("enabled")]
		public bool? Enabled { get; set; }

		[JsonProperty("loading")]
		[JsonConverter(typeof(StringEnumConverter))]
		public NormsLoading? Loading { get; set; }

	}
}